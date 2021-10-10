using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.Grue
{
    public class GrueCharacterCardController : HeroCharacterCardController
    {
        public GrueCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator e;
            switch(index)
            {
                case 0:
                case 1:
                case 2:
                default: yield break;
            }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            return this.PutDarknessIntoPlay(CharacterCard);
        }
    }
}
