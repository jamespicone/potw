using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Armsmaster
{
    public class CommunicatorCardController : ModuleCardController
    {
        public CommunicatorCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator DoPrimary()
        {
            //  "A hero other than Armsmaster may use a power"
            var e = GameController.SelectHeroToUsePower(HeroTurnTakerController, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        public override IEnumerator DoSecondary()
        {
            // "A hero other than Armsmaster may play a card"
            var e = SelectHeroToPlayCard(HeroTurnTakerController, heroCriteria: new LinqTurnTakerCriteria(tt => tt != TurnTaker));
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
