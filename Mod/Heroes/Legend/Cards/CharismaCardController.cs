using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Legend
{
    public class CharismaCardController : CardController
    {
        public CharismaCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            // Each hero may either regain 2 HP or play a card
            var e = EachPlayerSelectsFunction(
                httc => ! httc.IsIncapacitatedOrOutOfGame,
                httc => new Function[]
                {
                    new Function(httc, "Regain 2 HP", SelectionType.GainHP, () => RegainHP(httc)),
                    new Function(httc, "Play a card", SelectionType.PlayCard, () => PlayACard(httc))
                }
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

        private IEnumerator RegainHP(HeroTurnTakerController httc)
        {
            var e = GameController.SelectAndGainHP(
                httc,
                2,
                additionalCriteria: c => c.Owner == httc.TurnTaker && c.IsCharacter && c.IsTarget,
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

        private IEnumerator PlayACard(HeroTurnTakerController httc)
        {
            var e = GameController.SelectAndPlayCardFromHand(
                httc,
                optional: false,
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
    }
}
