﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.JessicaYamada
{
    public class JessicaYamadaCharacterTargetCardController : JessicaYamadaCharacterBase
    {
        public JessicaYamadaCharacterTargetCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
        }

        public override void AddSideTriggers()
        {
            base.AddSideTriggers();

            if (!Card.IsFlipped)
            {
                AddSideTrigger(AddTrigger<DealDamageAction>(
                    dda => dda.Target == Card && dda.DamageSource.Alignment().NonHero(),
                    dda => RedirectDamage(dda, TargetType.LowestHP, c => c != Card && c.Alignment().Hero().Target().Character() && !c.IsIncapacitatedOrOutOfGame && c.IsInPlay && c.IsRealCard),
                    TriggerType.RedirectDamage,
                    TriggerTiming.Before
                ));
            }
        }
    }
}
