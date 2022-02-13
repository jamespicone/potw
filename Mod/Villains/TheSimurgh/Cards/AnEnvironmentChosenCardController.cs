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
    public class AnEnvironmentChosenCardController : CardController, ISimurghDangerCard
    {
        public AnEnvironmentChosenCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public int Danger()
        {
            return 8;
        }

        public override IEnumerator Play()
        {
            // Play the top {H} cards of the environment deck.
            for (int i = 0; i < H; ++i)
            {
                var e = PlayTheTopCardOfTheEnvironmentDeckResponse(null);
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
}
