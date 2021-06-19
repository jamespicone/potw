using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Leviathan
{
    public class LeviathanTurnTakerController : TurnTakerController
    {
        public LeviathanTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {
        }

        public override IEnumerator StartGame()
        {
            // Reveal cards from the top of the villain deck until you reveal a Tactic card; put it into play.
            var stored = new List<RevealCardsAction>();
            var e = GameController.RevealCards(
                this,
                TurnTaker.Deck,
                c => c.DoKeywordsContain("tactic"),
                numberOfMatches: 1,
                stored,
                RevealedCardDisplay.ShowMatchingCards,
                CharacterCardController.GetCardSource()
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            if (stored.FirstOrDefault() != null)
            {
                if (stored.First().MatchingCards.FirstOrDefault() != null)
                {
                    var card = stored.First().MatchingCards.First();
                    e = GameController.PlayCard(
                        this,
                        card,
                        isPutIntoPlay: true,
                        responsibleTurnTaker: TurnTaker,
                        cardSource: CharacterCardController.GetCardSource()
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

            // Shuffle the other cards back into the villain deck
            e = GameController.ShuffleCardsIntoLocation(
                FindDecisionMaker(),
                TurnTaker.Revealed.Cards,
                TurnTaker.Deck,
                cardSource: CharacterCardController.GetCardSource()
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
