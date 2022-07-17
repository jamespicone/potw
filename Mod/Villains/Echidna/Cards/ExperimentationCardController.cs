using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Echidna
{
    public class ExperimentationCardController : CardController
    {
        public ExperimentationCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            // "If {EchidnaCharacter} is on her 'Mother of Monsters' side you may flip her to her Canny Tactician side.
            bool echidnaFlipped = false;
            IEnumerator e;
            if (CharacterCard.IsFlipped)
            {
                e = GameController.SendMessageAction(
                    $"{CharacterCard.Title} is already flipped; the top two cards of the Twisted deck will be played",
                    Priority.Medium, 
                    GetCardSource(),
                    new Card[] { CharacterCard },
                    showCardSource: true
                );
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }
            else
            {
                var results = new List<YesNoCardDecision>();
                e = GameController.MakeYesNoCardDecision(
                    DecisionMaker,
                    SelectionType.Custom,
                    CharacterCard,
                    storedResults: results,
                    associatedCards: new Card[] { Card },
                    cardSource: GetCardSource()
                );
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }

                echidnaFlipped = DidPlayerAnswerYes(results);
            }

            if (echidnaFlipped)
            {
                e = GameController.FlipCard(FindCardController(CharacterCard), cardSource: GetCardSource(), allowBackToFront: false);
            }
            else
            {
                // Otherwise, play the top two cards of the Twisted deck.
                e = GameController.PlayTopCardOfLocation(TurnTakerController, TurnTaker.FindSubDeck("TwistedDeck"), numberOfCards: 2);
            }
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            // Shuffle the villain trash into the villain deck.
            e = GameController.ShuffleTrashIntoDeck(TurnTakerController, cardSource: GetCardSource());
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {
            return new CustomDecisionText
            (
                "Flip Echidna?", "{0} is deciding whether to flip Echidna",
                "Should we flip Echidna?",
                "Flip Echidna"
            );
        }
    }
}
