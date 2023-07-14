using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Labyrinth
{
    public class CleanTheSlateCardController : CardController
    {
        public CleanTheSlateCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            // "You may destroy up to 3 environment cards.
            var storedDestroys = new List<DestroyCardAction>();
            var e = GameController.SelectAndDestroyCards(
                HeroTurnTakerController,
                new LinqCardCriteria(c => c.Is().Environment().Card(), "environment"),
                numberOfCards: 3,
                requiredDecisions: 0,
                storedResultsAction: storedDestroys,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            // "X on this card is the number of cards destroyed."
            var destroyedCount = GetNumberOfCardsDestroyed(storedDestroys);

            // "You may draw up to X cards.",
            e = DrawCards(HeroTurnTakerController, destroyedCount, optional: true, upTo: true);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            // "You may play X cards."
            e = SelectAndPlayCardsFromHand(
                HeroTurnTakerController,
                destroyedCount
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }
    }
}
