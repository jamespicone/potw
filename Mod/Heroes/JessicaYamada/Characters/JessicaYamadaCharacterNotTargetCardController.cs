using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.JessicaYamada
{
    public class JessicaYamadaCharacterNotTargetCardController : JessicaYamadaCharacterBase
    {
        public JessicaYamadaCharacterNotTargetCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
        }

        public override bool AskIfActionCanBePerformed(GameAction ga)
        {
            var ua = ga as UnincapacitateHeroAction;
            if (ua == null) { return true; }

            if (ua.HeroCharacterCard != this) { return true; }
            if (Card.IsFlipped) { return true; }

            return false;
        }
    }
}
