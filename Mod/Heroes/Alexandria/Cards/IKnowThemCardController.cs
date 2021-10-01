using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.Alexandria
{
    public class IKnowThemCardController : CardController
    {
        public IKnowThemCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowVillainTargetWithHighestHP();
        }

        public override IEnumerator Play()
        {
            // "{AlexandriaCharacter} deals 5 melee damage to the villain target with the highest HP"
            var results = new List<Card>();
            var e = GameController.FindTargetWithHighestHitPoints(
                1,
                c => c.Alignment(this).Villain().Target(),
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

            e = DealDamage(CharacterCard, highestVillain, amount: 5, DamageType.Melee, cardSource: GetCardSource());
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
