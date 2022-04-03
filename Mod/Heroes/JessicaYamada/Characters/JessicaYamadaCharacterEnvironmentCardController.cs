﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.JessicaYamada
{
    public class JessicaYamadaCharacterEnvironmentCardController : JessicaYamadaCharacterBase
    {
        public JessicaYamadaCharacterEnvironmentCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override void AddSideTriggers()
        {
            base.AddSideTriggers();

            if (! Card.IsFlipped)
            {
                AddSideTrigger(AddPreventDamageTrigger(dda => dda.Target == Card && dda.DamageSource.Is().Hero()));
            }
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            if (GameController.FindTargetsInPlay(c => c.Is().Hero().Target() && c.Location != TurnTaker.PlayArea).Count() <= 0) { return false; }
            return card == Card && card.HitPoints > 0;
        }
    }
}
