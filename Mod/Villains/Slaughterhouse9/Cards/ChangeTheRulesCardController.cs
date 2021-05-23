using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Slaughterhouse9
{
    public class ChangeTheRulesCardController : CardController
    {
        public ChangeTheRulesCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddTrigger<GameOverAction>(
                goa => goa.ResultIsVictory,
                CancelResponse,
                new TriggerType[] {
                    TriggerType.CancelAction,
                    TriggerType.GameOver
                },
                TriggerTiming.Before
            );
        }

        public override IEnumerator Play()
        {           
            // Shuffle all Nine targets in play back under the Slaughterhouse 9 card.
            // Move X cards from under the Slaughterhouse 9 card into the villain play area, where X = 1 + the number of Nine cards that were in play
            var nineCard = FindCard("Slaughterhouse9Character", realCardsOnly: false);
            if (nineCard == null) { yield break; }

            var selectedCard = new List<Card>();
            var e = GameController.FindTargetWithLowestHitPoints(
                ranking: 1,
                c => c.IsTarget && c.DoKeywordsContain("nine"),
                selectedCard,
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

            if (selectedCard.Count <= 0 || selectedCard.First() == null) { yield break; }

            e = GameController.ShuffleCardIntoLocation(DecisionMaker, selectedCard.First(), nineCard.UnderLocation, optional: false, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            if (selectedCard.First().MaximumHitPoints != null && selectedCard.First().Location == nineCard.UnderLocation)
            {
                e = GameController.SetHP(selectedCard.First(), selectedCard.First().MaximumHitPoints ?? 0, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }
            }

            if (nineCard.UnderLocation.TopCard == null) { yield break; }

            e = GameController.PlayCard(
                TurnTakerController,
                nineCard.UnderLocation.TopCard,
                isPutIntoPlay: true,
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

            if (nineCard.UnderLocation.TopCard == null) { yield break; }

            e = GameController.PlayCard(
                TurnTakerController,
                nineCard.UnderLocation.TopCard,
                isPutIntoPlay: true,
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
