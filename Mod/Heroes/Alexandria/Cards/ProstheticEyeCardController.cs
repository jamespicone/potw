using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Alexandria
{
    public class ProstheticEyeCardController : CardController
    {
        public ProstheticEyeCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            // "At the end of your turn reveal the top 3 cards of a deck. Put them back in any order"
            AddEndOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => RevealAndReplace(),
                TriggerType.RevealCard
            );
        }

        private IEnumerator RevealAndReplace()
        {
            var selectedDeck = new List<SelectLocationDecision>();
            var e = GameController.SelectADeck(
                HeroTurnTakerController,
                SelectionType.RevealCardsFromDeck,
                l => true,
                storedResults: selectedDeck,
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

            var deck = GetSelectedLocation(selectedDeck);
            if (deck == null) { yield break; }

            e = RevealTheTopCardsOfDeck_MoveInAnyOrder(
                HeroTurnTakerController,
                TurnTakerController,
                deck.OwnerTurnTaker,
                numberOfCardsToReveal: 3
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
