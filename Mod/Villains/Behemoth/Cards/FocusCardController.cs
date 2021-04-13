using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Behemoth
{
    public class FocusCardController : MovementCardController
    {
        public FocusCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroCharacterCardWithHighestHP(ranking: 1, numberOfTargets: 1);
        }

        public override IEnumerator Play()
        {
            // "The player whose hero has the highest HP puts 1 proximity token on their hero."
            List<Card> highest = new List<Card>();
            IEnumerator findCoroutine = base.GameController.FindTargetWithHighestHitPoints(1, (Card c) => c.IsHeroCharacterCard, highest, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(findCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(findCoroutine);
            }
            TurnTaker highestTT = highest.FirstOrDefault().Owner;
            IEnumerator addCoroutine = AddProximityTokens(highestTT, 1, GetCardSource(), true);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(addCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(addCoroutine);
            }
            //Log.Debug("FocusCardController.Play() finished, passing to base.Play()");
            yield return base.Play();
        }
    }
}
