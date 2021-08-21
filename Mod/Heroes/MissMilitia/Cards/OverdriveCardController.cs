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
            SpecialStringMaker.ShowNumberOfCardsInPlay(YourUniqueWeaponsInPlay(TurnTaker));
        }

        public override void AddTriggers()
        {
            AddTrigger<GameAction>(
                ga => ! (ga is SetPhaseActionCountAction) && ShouldUpdatePhaseCount(),
                ga => UpdatePhaseCount(),
                TriggerType.SetPhaseActionCount,
                TriggerTiming.After,
                outOfPlayTrigger: true
            );
        }

        public LinqCardCriteria YourUniqueWeaponsInPlay(TurnTaker owner)
        {
            return new LinqCardCriteria((c) => c.IsInPlayAndHasGameText && c.Owner == owner && c.DoKeywordsContain("weapon") && FindCardController(c).IsFirstOrOnlyCopyOfThisCardInPlay(), "unique Weapon");
        }

        public int NumberOfUniqueWeaponsInPlay(TurnTaker owner)
        {
            return FindCardsWhere(YourUniqueWeaponsInPlay(owner), visibleToCard: GetCardSource()).Count();
        }

        public override IEnumerator Play()
        {
            IEnumerator e;

            // "Count the number of your unique Weapon cards in play. You may use that many powers this turn."
            if (NumberOfUniqueWeaponsInPlay(TurnTaker) <= 0)
            {
                e = GameController.SendMessageAction(TurnTaker.Name + " has no Weapon cards in play, so they may not use powers this turn.", Priority.High, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }
            }

            // If played off-turn or after power step gives immediate power uses. Otherwise we add our marker status effect.
            // TODO: This doesn't work properly with strange phase orders. It matches Expatriette's Unload though. Possibly a base game bug?
            if (GameController.ActiveTurnTaker == TurnTaker && GameController.ActiveTurnPhase.Phase < Phase.UsePower)
            {
                var effect = new OnPhaseChangeStatusEffect(
                    CardWithoutReplacements,
                    nameof(NoOpEffect),
                    $"This turn {TurnTaker.Name} may use as many powers as they have unique Weapon cards in play",
                    new TriggerType[] { TriggerType.SetPhaseActionCount },
                    Card
                );

                effect.UntilEndOfPhase(TurnTaker, Phase.UsePower);
                effect.UntilThisTurnIsOver(Game);

                e = AddStatusEffect(effect);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }
            }
            else
            {
                var powersUsed = (from p in Journal.UsePowerEntriesThisTurn() where p.PowerUser == HeroTurnTaker select p).Count();
                while (powersUsed < NumberOfUniqueWeaponsInPlay(TurnTaker))
                {
                    var results = new List<UsePowerDecision>();
                    e = GameController.SelectAndUsePower(HeroTurnTakerController, storedResults: results, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(e);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(e);
                    }
                    ++powersUsed;

                    if (! WasPowerUsed(results))
                    {
                        break;
                    }
                }
            }

            // "At the end of this turn, destroy each Weapon card whose power you used this turn."
            OnPhaseChangeStatusEffect weaponExpiration = new OnPhaseChangeStatusEffect(
                CardWithoutReplacements,
                nameof(DestroyUsedWeaponsResponse),
                "At the end of this turn, destroy each Weapon card whose power " + TurnTaker.Name + " used this turn.",
                new TriggerType[] { TriggerType.DestroyCard },
                Card
            );

            weaponExpiration.BeforeOrAfter = BeforeOrAfter.After;
            weaponExpiration.TurnTakerCriteria.IsSpecificTurnTaker = Game.ActiveTurnTaker;
            weaponExpiration.TurnPhaseCriteria.Phase = Phase.End;
            weaponExpiration.TurnIndexCriteria.EqualTo = Game.TurnIndex;

            weaponExpiration.UntilThisTurnIsOver(Game);

            e = GameController.AddStatusEffect(weaponExpiration, true, GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        public IEnumerator DestroyUsedWeaponsResponse(PhaseChangeAction pca, OnPhaseChangeStatusEffect sourceEffect)
        {
            // "... destroy each Weapon card whose power you used this turn."
            var usedWeapons = (from p in Journal.UsePowerEntriesThisTurn() where p.PowerUser == HeroTurnTaker && p.CardWithPower.DoKeywordsContain("weapon") select p.CardWithPower);

            var e = GameController.DestroyCards(
                HeroTurnTakerController,
                new LinqCardCriteria(
                    c => usedWeapons.Contains(c),
                    "Weapon cards whose powers were used by " + TurnTaker.Name + " this turn",
                    useCardsSuffix: false,
                    singular: "Weapon card whose power was used by " + TurnTaker.Name + " this turn",
                    plural: "Weapon cards whose powers were used by " + TurnTaker.Name + " this turn"
                ),
                cardSource: GetCardSource()
            );

            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        private bool ShouldUpdatePhaseCount()
        {
            var myStatusEffects = GameController.StatusEffectManager.StatusEffectControllers.Select(sec => sec.StatusEffect).Where(se => se.CardSource == Card);
            foreach (var effect in myStatusEffects)
            {
                var turntaker = effect.FromTurnPhaseExpiryCriteria.TurnTaker;
                var phase = FindTurnPhase(turntaker, Phase.UsePower);
                if (phase == null)
                {
                    continue;
                }

                if (phase.GetPhaseActionCount() != NumberOfUniqueWeaponsInPlay(turntaker))
                {
                    return true;
                }
            }

            return false;
        }

        private IEnumerator UpdatePhaseCount()
        {
            var myStatusEffects = GameController.StatusEffectManager.StatusEffectControllers.Select(sec => sec.StatusEffect).Where(se => se.CardSource == Card);
            foreach (var effect in myStatusEffects)
            {
                var turntaker = effect.FromTurnPhaseExpiryCriteria.TurnTaker;
                var phase = FindTurnPhase(turntaker, Phase.UsePower);
                if (phase == null)
                {
                    continue;
                }

                var e = GameController.SetPhaseActionCount(phase, NumberOfUniqueWeaponsInPlay(turntaker), GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }
            }
        }

        public IEnumerator NoOpEffect(PhaseChangeAction p, OnPhaseChangeStatusEffect effect)
        {
            yield break;
        }
    }
}
