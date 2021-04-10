using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.NewDelhi
{
    public class LightningRodCardController : CardController
    {
        public LightningRodCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // "Whenever a target would be dealt lightning damage, redirect it to this card."
            AddRedirectDamageTrigger((DealDamageAction dda) => dda.DamageType == DamageType.Lightning && dda.Target != base.Card, () => base.Card);
            base.AddTriggers();
        }
    }
}
