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
    public class MushCardController : TheMerchantsUtilityCardController
    {
        public MushCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // "This card is immune to melee and projectile damage."
            AddImmuneToDamageTrigger((DealDamageAction dda) => dda.Target == base.Card && (dda.DamageType == DamageType.Melee || dda.DamageType == DamageType.Projectile));
            // "At the end of the villain turn, this card deals each hero target 1 melee damage."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => DealDamage(base.Card, (Card c) => c.IsHero, 1, DamageType.Melee), TriggerType.DealDamage);
            base.AddTriggers();
        }
    }
}
