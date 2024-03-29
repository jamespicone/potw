﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.TheSimurgh
{
    public class ACrucialHesitationCardController : CardController
    {
        public ACrucialHesitationCardController(Card card, TurnTakerController controller) : base(card, controller)
        {}

        public override void AddTriggers()
        {
            // "If this card ever has no tokens on it, the heroes lose the game.",
            AddTrigger<RemoveTokensFromPoolAction>(
                pa => pa.TokenPool.CardWithTokenPool == Card && pa.TokenPool.CurrentValue == 0,
                pa => SimurghSchemeSucceeded(pa),
                TriggerType.GameOver,
                TriggerTiming.After
            );

            AddTrigger<MoveTokensFromPoolToPoolAction>(
                pa => pa.FromTokenPool.CardWithTokenPool == Card && pa.FromTokenPool.CurrentValue == 0,
                pa => SimurghSchemeSucceeded(pa),
                TriggerType.GameOver,
                TriggerTiming.After
            );

            // "At the end of the villain turn, remove a token from this card.",
            AddEndOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => LoseAToken(pca),
                TriggerType.ModifyTokens
            );

            // At the end of each hero turn if they have done at most one of play a card / use a power / draw a card put a token on this card.
            AddEndOfTurnTrigger(
                tt => tt.IsHero,
                pca => AddAToken(),
                new TriggerType[] { TriggerType.AddTokensToPool },
                additionalCriteria: pca => DidPlayerOnlyDoOneThing(pca)
            );

            // Tell game that drawing isn't free
            AddTrigger<DrawCardAction>(
                dca => dca.IsSuccessful && dca.WillDrawCards && ItMattersIfTurnTakerDraws(dca.HeroTurnTaker),
                dca => DoNothing(),
                TriggerType.HiddenLast,
                TriggerTiming.Before
            );
        }

        public override IEnumerator Play()
        {
            // When this card is flipped face up, put {8 - H} tokens on it. 
            // We start with 8 tokens, lose H.
            var pool = Card.FindTokenPool("ACrucialHesitationPool");
            if (pool == null) { yield break; }

            var e = GameController.RemoveTokensFromPool(pool, H, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        private IEnumerator SimurghSchemeSucceeded(GameAction action)
        {
            return GameController.GameOver(
                EndingResult.AlternateDefeat,
                "The Simurgh's schemes have succeeded! Who knows what catastrophes she has arranged?",
                showEndingTextAsMessage: true,
                action,
                cardSource: GetCardSource()
            );
        }

        private IEnumerator LoseAToken(PhaseChangeAction pca)
        {
            var pool = Card.FindTokenPool("ACrucialHesitationPool");
            if (pool == null) { yield break; }

            var e = GameController.RemoveTokensFromPool(pool, 1, gameAction: pca, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        private IEnumerator AddAToken()
        {
            var pool = Card.FindTokenPool("ACrucialHesitationPool");
            if (pool == null) { yield break; }

            var e = GameController.AddTokensToPool(pool, 1, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        private bool ItMattersIfTurnTakerDraws(HeroTurnTaker heroTurnTaker)
        {
            // If it's not your turn it doesn't matter
            if (GameController.ActiveTurnTaker != heroTurnTaker) { return false; }

            var didDraw = Journal.DrawCardEntriesThisTurn().Count(j => j.Hero == heroTurnTaker) > 0;
            // Already drew, no reason not to draw more
            if (didDraw) { return false; }

            var didUsePower = Journal.UsePowerEntriesThisTurn().Count(j => j.PowerUser == heroTurnTaker) > 0;
            var didPlay = Journal.PlayCardEntriesThisTurn().Count(j => j.ResponsibleTurnTaker == heroTurnTaker && ! j.IsPutIntoPlay) > 0;

            // Already can't trigger the token add
            if (didUsePower && didPlay) { return false; }

            // Drawing won't prevent adding a token
            if (! (didUsePower || didPlay)) { return false; }

            // We've used a power xor played a card, so drawing or not drawing determines whether we trigger or not.
            return true;
        }

        private bool DidPlayerOnlyDoOneThing(PhaseChangeAction pca)
        {
            var didUsePower = Journal.UsePowerEntriesThisTurn().Count(j => j.PowerUser == pca.ToPhase.TurnTaker) > 0;
            var didDraw = Journal.DrawCardEntriesThisTurn().Count(j => j.Hero == pca.ToPhase.TurnTaker) > 0;
            var didPlay = Journal.PlayCardEntriesThisTurn().Count(j => j.ResponsibleTurnTaker == pca.ToPhase.TurnTaker && ! j.IsPutIntoPlay) > 0;

            return Convert.ToInt32(didUsePower) + Convert.ToInt32(didDraw) + Convert.ToInt32(didPlay) <= 1;
        }
    }
}
