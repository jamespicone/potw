using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Bitch
{
    public class HeelCardController : CardController
    {
        public HeelCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override System.Collections.IEnumerator Play()
        {
            var e = RevealCards_SelectSome_MoveThem_ReturnTheRest(
                HeroTurnTakerController,
                TurnTakerController,
                HeroTurnTaker.Deck,
                c => c.DoKeywordsContain("dog"),
                1, 1, false, true, true, "Dog"
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            e = DrawCard(HeroTurnTaker, optional: true);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            // TODO: Dog count in deck informational window
            // TODO: Does this shuffle?
        }
    }
}
