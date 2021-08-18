
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.JessicaYamada
{
    class AHelpfulSuggestionCardController : CardController
    {
        public AHelpfulSuggestionCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            // Another player may play a card and use a power, in any order
            var selectedPlayerList = new List<SelectTurnTakerDecision>();
            var e = GameController.SelectHeroTurnTaker(
                HeroTurnTakerController,
                SelectionType.PlayCard,
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

            e = SelectAndPerformFunction(
                selectedPlayerController,
                new[] {
                    new Function(selectedPlayerController, "Play a card", SelectionType.PlayCard, () => PlayCardFunc(selectedPlayerController)),
                    new Function(selectedPlayerController, "Use a power", SelectionType.UsePower, () => UsePowerFunc(selectedPlayerController))
                },
                associatedCards: new[] { Card }
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

        public IEnumerator PlayCardFunc(HeroTurnTakerController httc, bool powerUsed = false)
        {
            var e = SelectAndPlayCardFromHand(httc);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            if (! powerUsed)
            {
                e = UsePowerFunc(httc, true);
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

        public IEnumerator UsePowerFunc(HeroTurnTakerController httc, bool playUsed = false)
        {
            var e = GameController.SelectAndUsePower(httc, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            if (! playUsed)
            {
                e = PlayCardFunc(httc, true);
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
}
