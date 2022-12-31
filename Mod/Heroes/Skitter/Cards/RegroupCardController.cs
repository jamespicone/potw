using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Skitter
{
    public class RegroupCardController : CardController
    {
        public RegroupCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            // Place a bug token on Skitter."
            var e = this.AddBugTokenToSkitter(1);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            // You may move any number of bug tokens from one of your cards to {SkitterCharacter} or a Strategy card,
            // as many times as you would like.
            while (true)
            {
                // "You may move any number of bug tokens from one of your cards to {SkitterCharacter} or a Strategy card.",
                var didMove = new List<bool>();
                e = this.MoveBugTokens(moveArbitraryAmount: true, isOptional: true, didMove);
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }

                if (! didMove.FirstOrDefault())
                {
                    break;
                }
            }

            // You may play a card.
            e = SelectAndPlayCardFromHand(HeroTurnTakerController);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }
    }
}
