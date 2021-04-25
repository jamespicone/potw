using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Slaughterhouse9
{
    public class MannequinCharacterCardController : Slaughterhouse9MemberCharacterCardController
    {
        public MannequinCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
        }

        public override void AddSideTriggers()
        {
        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            // yuck
            base.AfterFlipCardImmediateResponse();

            yield break;
        }
    }
}
