﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Bitch
{
    public class GuardCardController : CardController
    {
        public GuardCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.DoKeywordsContain("dog"), "dog"));
        }

        public override IEnumerator Play()
        {
            // "Select up to X targets, where X = the number of Dog cards in play. Reduce damage dealt to those targets by 2 until the start of your next turn.",
            // "You may draw a card."
            var cards = GameController.FindCardsWhere(card => card.IsInPlay && card.DoKeywordsContain("dog"), true, GetCardSource());
            var dogCount = cards.Count();

            // TODO: Sort targets more helpfully
            var storedResults = new List<SelectCardsDecision>();
            var e = GameController.SelectCardsAndStoreResults(HeroTurnTakerController, SelectionType.ReduceDamageTaken, c => c.IsTarget && c.IsInPlayAndNotUnderCard, dogCount, storedResults, false, 0, cardSource: GetCardSource());
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            if (DidSelectCards(storedResults))
            {
                ReduceDamageStatusEffect status = new ReduceDamageStatusEffect(2);
                status.UntilStartOfNextTurn(TurnTaker);
                status.TargetCriteria.IsOneOfTheseCards = GetSelectedCards(storedResults).ToList();

                e = GameController.AddStatusEffect(status, true, GetCardSource());
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }

            e = DrawCard(HeroTurnTaker, true);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }
    }
}
