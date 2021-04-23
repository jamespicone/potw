using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Armsmaster
{
    public class RecyclerCardController : ModuleCardController
    {
        public RecyclerCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
        }

        public override IEnumerator DoPrimary()
        {
            // Draw a card
            var e = DrawCard(HeroTurnTaker);
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
            // Activate the [u]Secondary[/u] text of another Module attached to the card this card is attached tos
            var ownerCard = Card.Location.OwnerCard;
            if (ownerCard == null) { yield break; }

            var e = GameController.SelectAndActivateAbility(
                HeroTurnTakerController,
                "secondary",
                new LinqCardCriteria(c => c.DoKeywordsContain("module") && ownerCard.GetAllNextToCards(false).Contains(c) && c != Card),
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
