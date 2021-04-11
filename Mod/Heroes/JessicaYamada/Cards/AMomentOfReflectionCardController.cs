
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.JessicaYamada
{
    class AMomentOfReflectionCardController : CardController
    {
        public AMomentOfReflectionCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            // Another player may search their deck for a card, put it into their hand, and then shuffle their deck
            var selectedPlayerList = new List<SelectTurnTakerDecision>();
            var e = GameController.SelectHeroTurnTaker(
                HeroTurnTakerController,
                SelectionType.SearchDeck,
                optional: false,
                allowAutoDecide: false,
                storedResults: selectedPlayerList,
                heroCriteria: new LinqTurnTakerCriteria(tt => tt != TurnTaker),
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

            var selectedPlayer = GetSelectedTurnTaker(selectedPlayerList);
            if (selectedPlayer == null) { yield break; }

            var selectedPlayerController = FindTurnTakerController(selectedPlayer) as HeroTurnTakerController;
            if (selectedPlayerController == null) { yield break; }

            e = SearchForCards(
                selectedPlayerController,
                searchDeck: true,
                searchTrash: false,
                minNumberOfCards: 1,
                maxNumberOfCards: 1,
                new LinqCardCriteria(),
                putIntoPlay: false,
                putInHand: true,
                putOnDeck: false,
                shuffleAfterwards: true
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
