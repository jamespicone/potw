
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.JessicaYamada
{
    class StrategyAndSuggestionCardController : CardController
    {
        public StrategyAndSuggestionCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocations(() => from httc in base.GameController.FindHeroTurnTakerControllers() where !httc.IsIncapacitatedOrOutOfGame select httc.TurnTaker.Deck, new LinqCardCriteria((Card c) => c.DoKeywordsContain("ongoing"), "ongoing"));
        }

        public override IEnumerator Play()
        {
            /* "Another player reveals cards from their deck until they reveal an Ongoing.
             * They may either put it into play or into their hand. Shuffle the other revealed cards back into their deck",
             */
            var selectedPlayerList = new List<SelectTurnTakerDecision>();
            var e = GameController.SelectHeroTurnTaker(
                HeroTurnTakerController,
                SelectionType.RevealCardsFromDeck,
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

            var storedReveal = new List<RevealCardsAction>();
            e = GameController.RevealCards(
                selectedPlayerController,
                selectedPlayer.Deck,
                c => c.IsOngoing,
                1,
                storedReveal,
                RevealedCardDisplay.ShowMatchingCards,
                GetCardSource()
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            if (storedReveal.Count <= 0) { yield break; }

            foreach (var c in storedReveal.First().MatchingCards)
            {
                e = GameController.SelectLocationAndMoveCard(
                    selectedPlayerController,
                    c,
                    new MoveCardDestination[]
                    {
                        new MoveCardDestination(selectedPlayer.PlayArea),
                        new MoveCardDestination(selectedPlayerController.HeroTurnTaker.Hand)
                    },
                    isPutIntoPlay: true,
                    playIfMovingToPlayArea: true,
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

            e = CleanupRevealedCards(selectedPlayer.Revealed, selectedPlayer.Deck, shuffleAfterwards: true);
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
