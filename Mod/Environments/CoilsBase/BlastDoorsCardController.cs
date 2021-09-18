using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.CoilsBase
{
    public class BlastDoorsCardController : CardController
    {
        public BlastDoorsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // "Reduce all fire and projectile damage by X, where X is the number of times damage has been dealt this turn.",
            AddReduceDamageTrigger(dda => dda.DamageType == DamageType.Fire || dda.DamageType == DamageType.Projectile, dda => DamageCountThisTurn());

            // "At the start of the environment turn this card deals itself 1 irreducible melee damage"
            AddDealDamageAtStartOfTurnTrigger(TurnTaker, Card, c => c == Card, TargetType.All, amount: 1, DamageType.Melee, isIrreducible: true);
        }

        private int DamageCountThisTurn()
        {
            return Journal.DealDamageEntriesThisTurn().Count();
        }
    }
}
