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
            // Note: basing implementation on Mr. Fixer's Overdrive, rather than Expatriette's Unload, because Unload is implemented using CardCriteria which doesn't have a way of checking uniqueness
            if (base.GameController.ActiveTurnTaker == base.TurnTaker)
            {
                AllowSetNumberOfPowerUseStatusEffect overdriveStatus = new AllowSetNumberOfPowerUseStatusEffect(NumberOfUniqueWeaponsInPlay());
                overdriveStatus.UsePowerCriteria.CardSource = base.CharacterCard;
                overdriveStatus.UntilThisTurnIsOver(base.GameController.Game);
                overdriveStatus.CardDestroyedExpiryCriteria.Card = base.CharacterCard;
                overdriveStatus.NumberOfUses = 1;
                IEnumerator statusCoroutine = base.GameController.AddStatusEffect(overdriveStatus, true, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(statusCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(statusCoroutine);
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
