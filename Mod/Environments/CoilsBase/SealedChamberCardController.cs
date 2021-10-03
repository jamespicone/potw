using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.CoilsBase
{
    public class SealedChamberCardController : CardController
    {
        public SealedChamberCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // "If this card is destroyed, the contents escape. [b]GAME OVER.[/b]"
            AddWhenDestroyedTrigger(OnDestroyResponse, TriggerType.GameOver);

            // "At the start of their turn, a player may skip the rest of their turn. If they do, this card regains 5 HP."
            AddStartOfTurnTrigger(tt => this.HasAlignment(tt, CardAlignment.Hero), SkipTheirTurnToHealThisCardResponse, new TriggerType[] { TriggerType.SkipTurn, TriggerType.GainHP });
        }

        private IEnumerator OnDestroyResponse(DestroyCardAction dca)
        {
            // "... the contents escape. [b]GAME OVER.[/b]"
            var e = GameController.GameOver(
                EndingResult.EnvironmentDefeat,
                "The sealed chamber at the heart of Coil's base has been breached, and something is coming out...",
                showEndingTextAsMessage: true,
                actionSource: dca,
            cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        private IEnumerator SkipTheirTurnToHealThisCardResponse(PhaseChangeAction pca)
        {
            // "... a player may skip the rest of their turn. If they do, this card regains 5 HP."
            if (! pca.ToPhase.IsHero) { yield break; }

            
            var hero = GameController.FindHeroTurnTakerController(pca.ToPhase.TurnTaker.ToHero());
            var decision = new YesNoAmountDecision(GameController, hero, SelectionType.SkipTurn, 5, upTo: false, cardSource: GetCardSource());
            var e = GameController.MakeDecisionAction(decision);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            if (DidPlayerAnswerYes(decision))
            {
                e = GameController.GainHP(Card, 5, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }

                e = GameController.SkipToNextTurn(cardSource: GetCardSource());
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
