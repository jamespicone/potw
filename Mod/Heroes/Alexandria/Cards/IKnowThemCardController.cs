using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Alexandria
{
    public class IKnowThemCardController : CardController
    {
        public IKnowThemCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            // "{AlexandriaCharacter} deals 5 melee damage to the villain target with the highest HP"
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
            if (highestVillain == null) { yield break; }

            e = DealDamage(CharacterCard, highestVillain, amount: 5, DamageType.Psychic, cardSource: GetCardSource());
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
