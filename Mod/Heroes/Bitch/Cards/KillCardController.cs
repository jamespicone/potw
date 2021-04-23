using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Bitch
{
    public class KillCardController : CardController
    {
        public KillCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.DoKeywordsContain("dog"), "dog"));
        }

        public override System.Collections.IEnumerator Play()
        {
            // Deal a target X melee damage, where X = the number of Dog cards in play * 3
            // TODO: Make a dog do the damage

            var cards = GameController.FindCardsWhere(card => card.IsInPlay && card.DoKeywordsContain("dog"), true, GetCardSource());
            var dogCount = cards.Count();

            var e = GameController.SelectTargetsAndDealDamage(HeroTurnTakerController, new DamageSource(GameController, CharacterCard), 3 * dogCount, DamageType.Melee, 1, false, 1, cardSource: GetCardSource());
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
