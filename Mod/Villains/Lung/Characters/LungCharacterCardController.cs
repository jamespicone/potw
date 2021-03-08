using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Lung
{
    public class LungCharacterCardController : VillainCharacterCardController
    {
        /*
         "gameplay": [ "When the villain trash is shuffled into the villain deck, flip [i]Brute[/i], and flip {Lung}." ],
      "advanced": "At the end of the villain turn, discard the top card of the villain deck",
      "flippedGameplay": [
        "When flipped to this side, destroy all villain cards in play and put all cards in the villain deck into the villain trash.",
        "Villain cards cannot be played.",
        "Reduce damage dealt to Lung by 1."
      ],
      "flippedAdvanced": "When flipped to this side, the heroes lose the game",
        */
        public LungCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
        }

        public override void AddSideTriggers()
        {
            if (Card.IsFlipped)
            {
                AddSideTrigger(AddReduceDamageTrigger(c => c == Card, 1));
                AddSideTrigger(AddTrigger<PhaseChangeAction>(pca => pca.ToPhase.Phase == Phase.PlayCard && pca.ToPhase.TurnTaker.IsVillain, pca => GameController.SetPhaseActionCount(pca.ToPhase, null, GetCardSource()), TriggerType.SetPhaseActionCount, TriggerTiming.After));
                AddSideTrigger(AddTrigger<PlayCardAction>(pca => pca.CardToPlay.IsVillain && !pca.IsPutIntoPlay, pca => PreventVillainPlays(pca), TriggerType.CancelAction, TriggerTiming.Before));
            }
            else
            {
                AddSideTrigger(AddTrigger<ShuffleTrashIntoDeckAction>(sta => sta.TurnTakerController == TurnTakerController && sta.NecessaryToPlayCard, sta => FlipLungAndBrute(sta), TriggerType.FlipCard, TriggerTiming.After));
                
                if (IsGameAdvanced)
                {
                    AddSideTrigger(AddEndOfTurnTrigger(tt => tt == TurnTaker, pca => DiscardTopCard(), TriggerType.DiscardCard));
                }
            }
        }

        public IEnumerator FlipLungAndBrute(ShuffleTrashIntoDeckAction sta)
        {
            var e = CancelAction(sta);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            var flipCards = new List<CardController>();
            flipCards.Add(FindCardController(FindCard("BruteInstructions", realCardsOnly: false)));
            flipCards.Add(this);

            e = GameController.FlipCards(flipCards, GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            // yuck
            base.AfterFlipCardImmediateResponse();

            if (IsGameAdvanced)
            {
                var e2 = GameController.GameOver(EndingResult.AlternateDefeat, "Lung flipped", cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e2);
                }
                else
                {
                    GameController.ExhaustCoroutine(e2);
                }
            }

            var e = GameController.DestroyCards(DecisionMaker, new LinqCardCriteria(c => c.IsVillain && !c.IsCharacter), cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            e = GameController.BulkMoveCards(TurnTakerController, TurnTaker.Deck.Cards, TurnTaker.Trash, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        public IEnumerator PreventVillainPlays(PlayCardAction pca)
        {
            var e = CancelAction(pca);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }
    
        public IEnumerator DiscardTopCard()
        {
            var e = DiscardCardsFromTopOfDeck(TurnTakerController, 1, showMessage: true);
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
