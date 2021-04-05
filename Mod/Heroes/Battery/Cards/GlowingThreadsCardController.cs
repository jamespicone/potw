using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Battery
{
    public class GlowingThreadsCardController : BatteryUtilityCardController
    {
        public GlowingThreadsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // "Whenever {BatteryCharacter} uses her 'Discharge' power, reduce damage dealt to {BatteryCharacter} by 2 until the start of your next turn."
            AddTrigger<UsePowerAction>((UsePowerAction upa) => IsBatteryUsingDischargePower(upa), ReduceDamageTakenResponse, TriggerType.CreateStatusEffect, TriggerTiming.After);
            base.AddTriggers();
        }

        public IEnumerator ReduceDamageTakenResponse(UsePowerAction upa)
        {
            // "... reduce damage dealt to {BatteryCharacter} by 2 until the start of your next turn."
            ReduceDamageStatusEffect protectStatus = new ReduceDamageStatusEffect(2);
            protectStatus.UntilStartOfNextTurn(base.TurnTaker);
            protectStatus.TargetCriteria.IsSpecificCard = base.CharacterCard;
            protectStatus.CardSource = base.Card;

            IEnumerator protectCoroutine = AddStatusEffect(protectStatus, true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(protectCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(protectCoroutine);
            }
            yield break;
        }
    }
}
