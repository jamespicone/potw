using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Armsmaster
{
    public class StasisEffectorCardController : ModuleCardController
    {
        public StasisEffectorCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator DoPrimary()
        {
            // "Discard 2 cards. If you do, the next time a villain card would be played put it in the villain trash instead; and until the start of your next turn all villain character cards are immune to damage
            yield break;
        }

        public override IEnumerator DoSecondary()
        {
            // Destroy a target with 4 or less HP
            var e = GameController.SelectAndDestroyCard(HeroTurnTakerController, new LinqCardCriteria(c => c.IsTarget && c.HitPoints <= 4), optional: false, responsibleCard: Card, cardSource: GetCardSource());
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
