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
    public class ChangeTheRulesCardController : CardController
    {
        public ChangeTheRulesCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            // TODO: Honestly this is probably way too much; maybe should just shuffle the lowest HP active and deal 2 new ones?
            // Also make sure the villains don't lose the game when this happens

            // Shuffle all Nine targets in play back under the Slaughterhouse 9 card.
            // Move X cards from under the Slaughterhouse 9 card into the villain play area, where X = 1 + the number of Nine cards that were in play
            yield break;
        }
    }
}
