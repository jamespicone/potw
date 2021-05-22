using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Slaughterhouse9
{
    public class TheScreamCardController : CardController
    {
        public TheScreamCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            // Each hero may destroy any number of Equipment cards.
            var e = EachPlayerDestroysTheirCards(
                new LinqTurnTakerCriteria(),
                new LinqCardCriteria(c => c.DoKeywordsContain("equipment"), "equipment"),
                numberOfCards: null,
                requiredNumberOfCards: 0
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            // "The Nine card with the highest HP deals each hero target X projectile damage, where X = 2 * the number of Equipment cards in play"
            var selectedNine = new List<Card>();
            e = GameController.FindTargetWithHighestHitPoints(
                1,
                c => c.DoKeywordsContain("nine"),
                selectedNine,
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

            var NineSource = selectedNine.FirstOrDefault();
            if (NineSource == null) { yield break; }

            e = DealDamage(
                NineSource,
                c => c.IsHero && c.IsTarget,
                c => 2 * EquipmentCardCount(),
                DamageType.Projectile
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

        private int EquipmentCardCount()
        {
            return FindCardsWhere(new LinqCardCriteria(c => c.DoKeywordsContain("equipment") && c.IsInPlayAndHasGameText), GetCardSource()).Count();
        }
    }
}
