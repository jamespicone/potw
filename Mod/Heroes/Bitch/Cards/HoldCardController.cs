using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Bitch
{
    public class HoldCardController : CardController
    {
        public HoldCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.DoKeywordsContain("dog"), "dog"));
        }

        public override System.Collections.IEnumerator Play()
        {
            // Destroy a target with up to X HP, where X = the number of Dog cards in play * 2
            // You may draw a card
            var cards = GameController.FindCardsWhere(card => card.IsInPlay && card.DoKeywordsContain("dog"), true, GetCardSource());
            var dogCount = cards.Count();

            var e = GameController.SelectAndDestroyCard(
                HeroTurnTakerController,
                new LinqCardCriteria(c => c.IsTarget && c.HitPoints <= dogCount * 2),
                false
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            e = DrawCard(HeroTurnTaker, true);
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
