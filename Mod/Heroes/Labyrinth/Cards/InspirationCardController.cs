using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;
using UnityEngine;

namespace Jp.ParahumansOfTheWormverse.Labyrinth
{
    public class InspirationCardController : CardController
    {
        public InspirationCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            // Whenever an Environment card enters play, you may draw a card.
            AddTrigger<CardEntersPlayAction>(
                epa => epa.CardEnteringPlay.Is().Environment(),
                epa => DrawCard(HeroTurnTaker, optional: true),
                TriggerType.DrawCard,
                TriggerTiming.After
            );
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // Destroy an environment card.
            var storedDestroys = new List<DestroyCardAction>();
            var e = GameController.SelectAndDestroyCards(
                HeroTurnTakerController,
                new LinqCardCriteria(c => c.Is().Environment().Card(), "environment"),
                numberOfCards: 1,
                requiredDecisions: 1,
                storedResultsAction: storedDestroys,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            // If you do...
            if (GetNumberOfCardsDestroyed(storedDestroys) <= 0)
                yield break;

            // ...you may play a card.
            e = GameController.SelectAndPlayCardFromHand(
                HeroTurnTakerController,
                optional: true,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }
    }
}
