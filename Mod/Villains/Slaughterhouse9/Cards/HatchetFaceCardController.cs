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
    public class HatchetFaceCardController : CardController
    {
        public HatchetFaceCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // Heroes may not use powers
            AddPreventPhaseActionTrigger(Phase.UsePower, ttc => ttc.IsHero);
        }
    }
}
