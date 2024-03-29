﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Lung
{
    public class OniLeeCardController : CardController
    {
        public OniLeeCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowHeroTargetWithLowestHP(ranking: 1, numberOfTargets: 1);
            SpecialStringMaker.ShowNumberOfCardsAtLocation(base.TurnTaker.Deck, new LinqCardCriteria((Card c) => c.DoKeywordsContain("one-shot"), "one-shot"));
        }

        public override void AddTriggers()
        {
            // "At the end of the villain turn, {Oni Lee} deals the hero target with the lowest HP 3 melee damage.",
            // "Whenever {Oni Lee} would take damage, reveal the top card of the villain deck. If it is a one-shot prevent the damage. Shuffle the revealed card back into the villain deck"

            AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, c => c.Is(this).Hero().Target(), TargetType.LowestHP, 3, DamageType.Melee);
            AddTrigger<DealDamageAction>(
                dda => dda.Target == Card,
                dda => RevealAndPreventResponse(dda),
                new TriggerType[]{
                    TriggerType.RevealCard,
                    TriggerType.CancelAction
                },
                TriggerTiming.Before
            );
        }

        public IEnumerator RevealAndPreventResponse(DealDamageAction dda)
        {
            IEnumerator e;

            // Force ambiguous resolution
            if (GameController.PreviewMode)
            {
                e = GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.AmbiguousDecision, Card, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }
                yield break;
            }

            if (GameController.PretendMode)
            {
                yield break;
            }
            
            var storedResults = new List<Card>();
            e = GameController.RevealCards(TurnTakerController, TurnTaker.Deck, 1, storedResults, revealedCardDisplay: RevealedCardDisplay.ShowRevealedCards, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            var card = storedResults.FirstOrDefault();
            if (card != null && card.IsOneShot)
            {
                e = CancelAction(dda, isPreventEffect: true);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }
            }

            e = CleanupRevealedCards(TurnTaker.Revealed, TurnTaker.Deck, shuffleAfterwards: true);
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
