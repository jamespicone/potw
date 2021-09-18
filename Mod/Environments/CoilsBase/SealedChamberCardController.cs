using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            AddStartOfTurnTrigger(tt => tt.IsHero, SkipTheirTurnToHealThisCardResponse, new TriggerType[] { TriggerType.SkipTurn, TriggerType.GainHP });
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
            if (pca.ToPhase.IsHero)
            {
                HeroTurnTakerController hero = base.GameController.FindHeroTurnTakerController(pca.ToPhase.TurnTaker.ToHero());
                YesNoAmountDecision decision = new YesNoAmountDecision(base.GameController, hero, SelectionType.SkipTurn, 5, upTo: false, cardSource: GetCardSource());
                IEnumerator decideCoroutine = base.GameController.MakeDecisionAction(decision);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(decideCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(decideCoroutine);
                }
                if (DidPlayerAnswerYes(decision))
                {
                    IEnumerator healCoroutine = base.GameController.GainHP(base.Card, 5, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(healCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(healCoroutine);
                    }
                    IEnumerator skipCoroutine = base.GameController.SkipToNextTurn(cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(skipCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(skipCoroutine);
                    }
                }
            }
            yield break;
        }
    }
}
