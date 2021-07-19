using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Alexandria
{
    public class ColdReadingCardController : CardController
    {
        public ColdReadingCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            //"When this card enters play reveal the top 3 cards of the villain deck. Put them back in any order",
            var storedResults = new List<SelectLocationDecision>();
            var e = FindVillainDeck(
                HeroTurnTakerController,
                SelectionType.RevealCardsFromDeck,
                storedResults,
                l => true
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            var selectedLocation = GetSelectedLocation(storedResults);
            if (selectedLocation == null) { yield break; }

            e = RevealTheTopCardsOfDeck_MoveInAnyOrder(
                HeroTurnTakerController,
                TurnTakerController,
                selectedLocation.OwnerTurnTaker,
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

        public override void AddTriggers()
        {
            //"At the start of your turn reveal the top card of the villain deck. Either discard it or return it to the top of the villain deck"
            AddStartOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => RevealVillainCard(),
                TriggerType.RevealCard
            );
        }

        private IEnumerator RevealVillainCard()
        {
            var storedResults = new List<SelectLocationDecision>();
            var e = FindVillainDeck(
                HeroTurnTakerController,
                SelectionType.RevealCardsFromDeck,
                storedResults,
                l => true
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            var selectedLocation = GetSelectedLocation(storedResults);
            if (selectedLocation == null) { yield break; }

            e = RevealCard_DiscardItOrPutItOnDeck(
                HeroTurnTakerController,
                TurnTakerController,
                selectedLocation,
                toBottom: false
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
