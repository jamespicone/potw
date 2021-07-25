﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Jp.ParahumansOfTheWormverse.JessicaYamada
{
    public class JessicaYamadaCharacterEnvironmentCardController : JessicaYamadaCharacterBase
    {
        public JessicaYamadaCharacterEnvironmentCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
        }

        public override void AddSideTriggers()
        {
            base.AddSideTriggers();

            if (! Card.IsFlipped)
            {
                AddSideTrigger(AddPreventDamageTrigger(dda => dda.Target == Card && dda.DamageSource.IsHero));
            }
        }
    }
}