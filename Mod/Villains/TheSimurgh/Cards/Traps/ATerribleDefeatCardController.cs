using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.TheSimurgh
{
    public class ATerribleDefeatCardController : CardController
    {
        public ATerribleDefeatCardController(Card card, TurnTakerController controller) : base(card, controller)
        {}

        public override IEnumerator Play()
        {
            // When this card is flipped face up destroy every non-character card owned by the hero with the most cards in play.
            var storedResults = new List<TurnTaker>();
            var e = FindHeroWithMostCardsInPlay(
                storedResults,
                evenIfCannotDealDamage: true
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            var taker = storedResults.FirstOrDefault();
            if (taker == null) { yield break; }

            e = GameController.DestroyCards(
                DecisionMaker,
                new LinqCardCriteria(c => c.Owner == taker && !c.IsCharacter),
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
