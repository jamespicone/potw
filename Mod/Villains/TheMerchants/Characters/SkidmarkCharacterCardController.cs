using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;
using UnityEngine;

namespace Jp.ParahumansOfTheWormverse.TheMerchants
{
    class SkidmarkCharacterCardController : VillainCharacterCardController
    {
        public SkidmarkCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.GetThugDeck(), showInEffectsList: () => true);
        }

        public override bool CanBeDestroyed => false;

        public override void AddSideTriggers()
        {
            if (Card.IsFlipped)
            {
                // At the end of the villain turn, play the top card of the Thug deck.
                AddSideTrigger(AddEndOfTurnTrigger(tt => tt == TurnTaker, pca => this.PlayThugs(), TriggerType.PlayCard));
            }
            else
            {
                // At the end of the villain turn, play the top {H - 1} cards of the Thug deck.
                AddSideTrigger(AddEndOfTurnTrigger(tt => tt == TurnTaker, pca => this.PlayThugs(H - 1), TriggerType.PlayCard));

                if (IsGameAdvanced)
                {
                    // Reduce damage dealt to villain targets by 1.
                    AddSideTrigger(AddReduceDamageTrigger(c => c.Is(this).Villain().Target(), 1));
                }
            }

            // At the start of the villain turn, if the Thug deck is empty, the Merchants have grown out of control. [b]GAME OVER.[/b]
            AddSideTrigger(AddStartOfTurnTrigger(tt => tt == TurnTaker, GameOverResponse, TriggerType.GameOver, additionalCriteria: pca => TurnTaker.GetThugDeck().IsEmpty));
        }

        public override IEnumerator DestroyAttempted(DestroyCardAction destroyCard)
        {
            // If some effect is trying to destroy flipped Skidmark, just don't do it.
            if (Card.IsFlipped) yield break;

            // When {SkidmarkCharacter} would be destroyed, flip {SkidmarkCharacter}'s villain character card instead.
            var e = GameController.FlipCard(this, cardSource: GetCardSource());
            if (UseUnityCoroutines) yield return GameController.StartCoroutine(e);
            else GameController.ExhaustCoroutine(e);
        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            // Call super is a dumb pattern but this is what resets triggers.
            IEnumerator e = base.AfterFlipCardImmediateResponse();
            if (UseUnityCoroutines) yield return GameController.StartCoroutine(e);
            else GameController.ExhaustCoroutine(e);

            if (Card.IsFlipped) e = GameController.RemoveTarget(Card, cardSource: GetCardSource());
            else e = GameController.MakeTargettable(Card, 45, 45, GetCardSource());

            if (UseUnityCoroutines) yield return GameController.StartCoroutine(e);
            else GameController.ExhaustCoroutine(e);
        }

        public IEnumerator GameOverResponse(GameAction ga)
        {
            // the Merchants have grown out of control. GAME OVER.
            var e = GameController.GameOver(
                EndingResult.AlternateDefeat,
                "The Thug deck is empty! The Merchants have become too numerous for the heroes to contain!",
                showEndingTextAsMessage: true,
                actionSource: ga,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) yield return GameController.StartCoroutine(e);
            else GameController.ExhaustCoroutine(e);
        }
    }
}
