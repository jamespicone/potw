using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Alexandria
{
    public class AndTheyKnowMeCardController : CardController
    {
        public AndTheyKnowMeCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            // The villain target with the highest HP deals {AlexandriaCharacter} 5 psychic damage
            var results = new List<Card>();
            var e = GameController.FindTargetWithHighestHitPoints(
                1,
                c => c.IsVillain,
                results,
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

            var highestVillain = results.FirstOrDefault();
            if (highestVillain != null)
            {
                e = DealDamage(highestVillain, CharacterCard, amount: 5, DamageType.Psychic, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }
            }

            // The villain target with the highest HP cannot deal damage until the start of your next turn
            results = new List<Card>();
            e = GameController.FindTargetWithHighestHitPoints(
                1,
                c => c.IsVillain,
                results,
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

            highestVillain = results.FirstOrDefault();
            if (highestVillain == null) { yield break; }

            var effect = new CannotDealDamageStatusEffect();
            effect.UntilStartOfNextTurn(TurnTaker);
            effect.TargetCriteria.IsSpecificCard = highestVillain;

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
    }
}
