﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Slaughterhouse9
{
    public class ShatterbirdCharacterCardController : Slaughterhouse9MemberCharacterCardController
    {
        public ShatterbirdCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
        }

        public override void AddSideTriggers()
        {
            if (Card.IsFlipped)
            {
                // "At the end of the villain turn this card deals 1 projectile damage to all non-villain targets"
                AddSideTrigger(AddDealDamageAtEndOfTurnTrigger(
                    TurnTaker,
                    Card,
                    c => c.Is(this).NonVillain().Target(),
                    TargetType.All,
                    1,
                    DamageType.Projectile
                ));
            }
            else
            {
                // "The first time an Attack card would enter the trash each turn Shatterbird deals 2 projectile damage to the H - 1 hero targets with the highest HP"
                AddSideTrigger(AddAttackTrigger(
                    () => GlassAttack(),
                    new TriggerType[] { TriggerType.DealDamage },
                    "ShatterbirdAttack"
                ));
            }
        }

        public IEnumerator GlassAttack()
        {
            var e = DealDamageToHighestHP(
                Card,
                1,
                c => c.Is(this).Hero().Target(),
                c => 2,
                DamageType.Projectile,
                numberOfTargets: () => H - 1
            );

            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }
    }
}
