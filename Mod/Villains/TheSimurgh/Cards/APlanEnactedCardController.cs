using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.TheSimurgh
{
    public class APlanEnactedCardController : CardController, ISimurghDangerCard
    {
        public APlanEnactedCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public int Danger()
        {
            return 1;
        }

        public override IEnumerator Play()
        {
            // If there are no face-down villain cards in play
            IEnumerator e;
            var faceDownVillains = FindCardsWhere(c => c.IsFaceDownNonCharacter && c.Is().Villain().AccordingTo(this) && c.IsInPlay);
            if (faceDownVillains.Count() <= 0)
            {
                // ...play the top card of the villain deck.
                e = GameController.PlayTopCardOfLocation(TurnTakerController, TurnTaker.Deck, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }
                _trashMe = true;
            }
            else
            {
                // Otherwise, flip a face-down villain card, then shuffle this card into the villain deck
                e = GameController.SelectAndFlipCards(
                    DecisionMaker,
                    new LinqCardCriteria(c => c.IsFaceDownNonCharacter && c.Is().Villain().AccordingTo(this), "face-down villain"),
                    toFaceDown: false,
                    treatAsPutIntoPlay: true,
                    cardSource: GetCardSource()
                );
                _trashMe = false;
            }

            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            if (! _trashMe)
            {
                e = GameController.ShuffleCardIntoLocation(
                    DecisionMaker,
                    Card,
                    TurnTaker.Deck,
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

        public override bool DoNotMoveOneShotToTrash => ! _trashMe;
        private bool _trashMe = false;
    }
}
