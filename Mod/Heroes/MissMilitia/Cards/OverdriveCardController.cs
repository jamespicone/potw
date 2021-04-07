using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.MissMilitia
{
    public class OverdriveCardController : MissMilitiaUtilityCardController
    {
        public OverdriveCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(YourUniqueWeaponsInPlay(), owners: new TurnTaker[] { base.TurnTaker });
        }

        public LinqCardCriteria YourUniqueWeaponsInPlay()
        {
            return new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("weapon") && FindCardController(c).IsFirstOrOnlyCopyOfThisCardInPlay(), "unique Weapon");
        }

        public int NumberOfUniqueWeaponsInPlay()
        {
            return FindCardsWhere(YourUniqueWeaponsInPlay(), visibleToCard: GetCardSource()).Count();
        }

        public override IEnumerator Play()
        {
            // "Count the number of your unique Weapon cards in play. You may use that many powers this turn."
            if (NumberOfUniqueWeaponsInPlay() <= 0)
            {
                IEnumerator messageCoroutine = base.GameController.SendMessageAction(base.TurnTaker.Name + " has no Weapon cards in play, so they may not use powers this turn.", Priority.High, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(messageCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(messageCoroutine);
                }
            }
            // Note: basing implementation on cards like Flame Spike or Smite the Transgressor but with variable amount of powers, because Unload is implemented using CardCriteria which doesn't have a way of checking uniqueness
            if (base.GameController.ActiveTurnTaker == base.TurnTaker)
            {
                TurnPhase powerPhase = (from TurnPhase phase in GameController.ActiveTurnTaker.ToHero().TurnPhases where phase.IsUsePower select phase).FirstOrDefault();
                int additionalPowers = NumberOfUniqueWeaponsInPlay();
                Log.Debug(base.TurnTaker.Name + " has " + NumberOfUniqueWeaponsInPlay().ToString() + " unique Weapons in play.");
                if (powerPhase == null)
                {
                    Log.Debug("Couldn't find power phase from ActiveTurnTaker.");
                }
                else if (!powerPhase.GetPhaseActionCount().HasValue)
                {
                    Log.Debug("Found power phase from ActiveTurnTaker, but GetPhaseActionCount() returned " + powerPhase.GetPhaseActionCount().ToString());
                }
                if (powerPhase != null && powerPhase.GetPhaseActionCount().HasValue)
                {
                    Log.Debug("Current number of powers this turn: " + powerPhase.GetPhaseActionCount().Value.ToString());
                    additionalPowers -= powerPhase.GetPhaseActionCount().Value;
                    int totalPowers = additionalPowers + powerPhase.GetPhaseActionCount().Value;
                    Log.Debug("Granting " + additionalPowers.ToString() + " bonus powers for a total of " + totalPowers.ToString() + ".");
                }
                IEnumerator powersCoroutine = AdditionalPhaseActionThisTurn(base.TurnTaker, Phase.UsePower, additionalPowers);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(powersCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(powersCoroutine);
                }
            }
            else
            {
                int powersUsed = (from e in base.Journal.UsePowerEntriesThisTurn() where e.PowerUser == base.HeroTurnTaker select e).Count();
                if (powersUsed < NumberOfUniqueWeaponsInPlay())
                {
                    int powersRemaining = NumberOfUniqueWeaponsInPlay() - powersUsed;
                    IEnumerator powerCoroutine = base.GameController.SelectAndUsePower(base.HeroTurnTakerController, numberOfPowers: powersRemaining, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(powerCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(powerCoroutine);
                    }
                }
            }
            // "At the end of this turn, destroy each Weapon card whose power you used this turn."
            OnPhaseChangeStatusEffect weaponExpiration = new OnPhaseChangeStatusEffect(base.CardWithoutReplacements, nameof(DestroyUsedWeaponsResponse), "At the end of this turn, destroy each Weapon card whose power " + base.TurnTaker.Name + " used this turn.", new TriggerType[] { TriggerType.DestroyCard }, base.Card);
            weaponExpiration.NumberOfUses = 1;
            weaponExpiration.BeforeOrAfter = BeforeOrAfter.After;
            weaponExpiration.TurnTakerCriteria.IsSpecificTurnTaker = Game.ActiveTurnTaker;
            weaponExpiration.TurnPhaseCriteria.Phase = Phase.End;
            weaponExpiration.TurnIndexCriteria.EqualTo = base.Game.TurnIndex;
            IEnumerator expireCoroutine = base.GameController.AddStatusEffect(weaponExpiration, true, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(expireCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(expireCoroutine);
            }
            yield break;
        }

        public IEnumerator DestroyUsedWeaponsResponse(PhaseChangeAction pca, OnPhaseChangeStatusEffect sourceEffect)
        {
            // "... destroy each Weapon card whose power you used this turn."
            Log.Debug("Overdrive activating DestroyUsedWeaponsResponse");
            List<Card> usedWeapons = (from e in base.Journal.UsePowerEntriesThisTurn() where e.PowerUser == base.HeroTurnTaker && e.CardWithPower.DoKeywordsContain("weapon") select e.CardWithPower).ToList();
            foreach (Card weapon in usedWeapons)
            {
                Log.Debug(weapon.Title + " was used this turn and will be destroyed.");
            }
            IEnumerator destroyCoroutine = base.GameController.DestroyCards(base.HeroTurnTakerController, new LinqCardCriteria((Card c) => usedWeapons.Contains(c), "Weapon cards whose powers were used by " + base.TurnTaker.Name + " this turn", false, false, "Weapon card whose power was used by " + base.TurnTaker.Name + " this turn", "Weapon cards whose powers were used by " + base.TurnTaker.Name + " this turn"), cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(destroyCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(destroyCoroutine);
            }
            yield break;
        }
    }
}
