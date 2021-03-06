﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Lung
{
    public class WingsCardController : CardController
    {
        public WingsCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            // {Lung} is immune to damage from Environment cards
            // TODO: Should I be checking identifiers?
            AddImmuneToDamageTrigger(dda => dda.DamageSource.IsEnvironmentSource && dda.Target == TurnTaker.CharacterCard);
        }
    }
}
