using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Lung
{
    public class SmashCardController : CardController
    {
        public SmashCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowHeroTargetWithLowestHP(ranking: 1, numberOfTargets: 1);
        }

        public override System.Collections.IEnumerator Play()
        {
            // "{Lung} deals the hero target with the lowest HP 2 melee damage.",
            // "Destroy 1 hero ongoing or equipment card"
            var e = DealDamageToLowestHP(TurnTaker.CharacterCard, 1, c => c.IsHero && c.IsTarget && c.IsInPlay, c => 2, DamageType.Melee);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            e = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria(c => c.IsHero && (c.IsOngoing || c.DoKeywordsContain("equipment"))), optional: false, responsibleCard: Card, cardSource: GetCardSource());
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
