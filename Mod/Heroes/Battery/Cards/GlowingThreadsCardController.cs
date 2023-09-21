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
    public class GlowingThreadsCardController : CardController
    {
        public GlowingThreadsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // Whenever {BatteryCharacter} uses her 'Discharge' power...
            AddTrigger<UsePowerAction>(
                upa => GameController.IsDischargePower(upa, HeroTurnTaker),
                ReduceDamageTakenResponse,
                TriggerType.CreateStatusEffect,
                TriggerTiming.After
            );
        }

        public IEnumerator ReduceDamageTakenResponse(UsePowerAction upa)
        {
            // ... reduce damage dealt to {BatteryCharacter} by 2 until the start of your next turn.
            var effect = new ReduceDamageStatusEffect(2);
            effect.UntilStartOfNextTurn(TurnTaker);
            effect.TargetCriteria.IsSpecificCard = CharacterCard;

            var e = AddStatusEffect(effect, true);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }
    }
}
