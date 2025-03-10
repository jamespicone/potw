using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Legend
{
    public class SplitshotCardController : CardController
    {
        public SplitshotCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            AddThisCardControllerToList(CardControllerListType.CanCauseDamageOutOfPlay);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // Choose an effect, then apply it to up to 3 targets
            var numberOfTargets = GetPowerNumeral(0, 3);

            var targets = new List<Card>();
            var effects = new List<IEffectCardController>();

            var e = this.ChooseEffects(effects);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            var possibleTargets = FindCardsWhere(
                c => c.IsTarget && c.IsInPlay,
                realCardsOnly: true,
                visibleToCard: GetCardSource()
            );

            var damage = new List<DealDamageAction>();
            foreach (var effect in effects)
            {
                damage.Add(effect.TypicalDamageAction(targets, this, GetCardSource()));
            }

            var decision = new SelectTargetsDecision(
                GameController,
                HeroTurnTakerController,
                c => possibleTargets.Contains(c),
                numberOfCards: numberOfTargets,
                requiredDecisions: 0,
                damageSource: damage.First().DamageSource,
                amount: damage.First().Amount,
                damageType: damage.First().DamageType,
                isIrreducible: damage.First().IsIrreducible,
                followUpDamageInformation: damage.Skip(1),
                addStatusEffect: damage.First().StatusEffectResponses.FirstOrDefault(),
                selectTargetsEvenIfCannotPerformAction: true,
                cardSource: GetCardSource()
            );

            decision.SequenceIndex = 1;

            e = GameController.SelectCardsAndDoAction(decision, scd => DoNothing(), cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            targets.AddRange(GetSelectedCards(new SelectCardsDecision[] { decision }));

            e = this.ApplyEffects(effects, targets, EffectTargetingOrdering.OrderingAlreadyDecided, GetCardSource());
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
}
