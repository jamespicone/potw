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
        { }

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
                damage.Add(effect.TypicalDamageAction(targets));
            }

            for (int i = 0; i < numberOfTargets; ++i)
            {
                var storedResult = new List<SelectCardDecision>();
                e = GameController.SelectCardAndStoreResults(
                    HeroTurnTakerController,
                    SelectionType.SelectTarget,
                    possibleTargets,
                    storedResult,
                    dealDamageInfo: damage,
                    optional: true,
                    selectionTypeOrdinal: i + 1,
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

                var card = GetSelectedCard(storedResult);
                if (card == null) { break; }

                targets.Add(card);
            }

            e = this.ApplyEffects(effects, targets, EffectTargetingOrdering.OrderingAlreadyDecided);
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
