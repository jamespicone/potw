using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

using Handelabra;

namespace Jp.ParahumansOfTheWormverse.Echidna
{
    public class AbhorrorTwistedCardController : CardController
    {
        public AbhorrorTwistedCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowLowestHP(cardCriteria: new LinqCardCriteria(c => AskIfCardContainsKeyword(c, "twisted"), "Twisted"));
        }

        public override void AddTriggers()
        {
            // Reduce damage dealt to villain targets by 1.
            AddReduceDamageTrigger(c => c.Is().Villain().Target().AccordingTo(this), 1);

            // At the end of the villain turn...
            AddEndOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => PlayAndReturn(pca),
                new TriggerType[] {
                    TriggerType.PutIntoPlay,
                    TriggerType.ShuffleCardIntoDeck
                }
            );
        }

        private IEnumerator PlayAndReturn(PhaseChangeAction pca)
        {
            // ...put the top card of the Twisted deck into play...
            var e = GameController.PlayTopCardOfLocation(
                TurnTakerController,
                TurnTaker.FindSubDeck("TwistedDeck"),
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            // ...then shuffle the Twisted with the lowest HP back into the Twisted deck.
            var lowest = new List<Card>();
            e = GameController.FindTargetWithLowestHitPoints(
                1,
                c => AskIfCardContainsKeyword(c, "twisted"),
                lowest,
                pca,
                evenIfCannotDealDamage: true,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            if (lowest.FirstOrDefault() == null) { yield break; }

            e = GameController.ShuffleCardIntoLocation(
                DecisionMaker,
                lowest.First(),
                TurnTaker.FindSubDeck("TwistedDeck"),
                optional: false,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }
    }
}
