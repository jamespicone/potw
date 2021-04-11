
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.JessicaYamada
{
    class HelpNoMatterTheCostCardController : CardController
    {
        public HelpNoMatterTheCostCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            // Discard any number of cards. Another player draws X cards, where X is the number of cards discarded + 2.
            var discardResultList = new List<DiscardCardAction>();
            var e = SelectAndDiscardCards(
                HeroTurnTakerController,
                numberOfCardsToDiscard: null,
                requiredDecisions: 0,
                storedResults: discardResultList
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            var numberToDraw = GetNumberOfCardsDiscarded(discardResultList) + 2;
            e = GameController.SelectHeroToDrawCards(
                HeroTurnTakerController,
                numberToDraw,
                additionalCriteria: new LinqTurnTakerCriteria(tt => tt != TurnTaker),
                cardSource: GetCardSource()
            );
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
