using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.TheMerchants
{
    public class HelicopterCardController : CardController
    {
        public HelicopterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // "{SkidmarkCharacter} is immune to melee damage."
            AddImmuneToDamageTrigger(dda => dda.Target == CharacterCard && dda.DamageType == DamageType.Melee);

            // "At the end of the villain turn, play the top card of the Thug deck."
            AddEndOfTurnTrigger(tt => tt == TurnTaker, 
                pca => this.PlayThugs(),
                TriggerType.PlayCard
            );
        }
    }
}
