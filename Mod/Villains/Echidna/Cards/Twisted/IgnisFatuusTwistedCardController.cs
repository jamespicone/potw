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
    public class IgnisFatuusTwistedCardController : CardController
    {
        public IgnisFatuusTwistedCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            var pool = Card.FindTokenPool("PowerPool");
            if (pool != null)
            {
                SpecialStringMaker.ShowTokenPool(pool, this, "There {0} {1} Power {2} on " + Card.Title);
            }
        }

        public override void AddTriggers()
        {
            // At the end of the villain turn this card deals 5 psychic damage to all hero targets.
            AddDealDamageAtEndOfTurnTrigger(
                TurnTaker,
                Card,
                c => c.Is().Hero().Target(),
                TargetType.All,
                5,
                DamageType.Psychic
            );

            // At the start of the villain turn,
            // if there's a Power token on this card,
            // Ignis Fatuus has reached full power. [b]GAME OVER[/b].
            // Otherwise, put a Power token on this card.
            AddStartOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => CheckForLoss(pca),
                new TriggerType[] { TriggerType.GameOver, TriggerType.AddTokensToPool }
            );

            AddAfterLeavesPlayAction(() => ResetTokenPool());
        }

        public override IEnumerator Play()
        {
            // If this card would enter play during setup...
            if (Game.HasGameStarted) { yield break; }

            // put the top card of the Twisted deck into play...
            var e = GameController.PlayTopCardOfLocation(
                TurnTakerController,
                TurnTaker.FindSubDeck("TwistedDeck"),
                isPutIntoPlay: true
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            // then shuffle this card back into the Twisted deck.
            e = GameController.ShuffleCardIntoLocation(
                DecisionMaker,
                Card,
                TurnTaker.FindSubDeck("TwistedDeck"),
                optional: false,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }

        private IEnumerator ResetTokenPool()
        {
            var pool = Card.FindTokenPool("PowerPool");
            if (pool == null) { yield break; }

            pool.SetToInitialValue();
            yield break;
        }
        private IEnumerator CheckForLoss(PhaseChangeAction pca)
        {
            var pool = Card.FindTokenPool("PowerPool");
            if (pool == null) { yield break; }

            if (pool.CurrentValue > 0)
            {
                var e = GameController.GameOver(
                    EndingResult.AlternateDefeat,
                    "Ignis Fatuus has reached full power!",
                    showEndingTextAsMessage: true,
                    actionSource: pca,
                    cardSource: GetCardSource()
                );
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }
            else
            {
                var e = GameController.AddTokensToPool(pool, 1, GetCardSource());
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }
        }
    }
}
