using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.TheSimurgh
{
    public class ACapabilityRevealedCardController : CardController, ISimurghDangerCard
    {
        public ACapabilityRevealedCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public int Danger()
        {
            return 3;
        }

        public override IEnumerator Play()
        {
            // Destroy all Equipment cards.
            var destroyed = new List<DestroyCardAction>();
            var e = GameController.DestroyCards(
                DecisionMaker,
                new LinqCardCriteria(c => c.DoKeywordsContain("equipment"), "equipment"),
                storedResults: destroyed,
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

            // Reveal the top X cards of the villain deck, where X is the number of cards destroyed this way. Put the revealed cards back in ascending order of {SimurghDanger}."
            e = TurnTakerController.StackDeck(GetDestroyedCards(destroyed).Count(), GetCardSource());
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
