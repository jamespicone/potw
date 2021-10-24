using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.TheSimurgh
{
    public class AThreatForeseenCardController : CardController, ISimurghDangerCard
    {
        public AThreatForeseenCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public int Danger()
        {
            // TODO
            return 0;
        }

        public override void AddTriggers()
        {
            // The first time {TheSimurghCharacter} would be dealt damage each round, redirect that damage to the hero with the lowest HP.
            AddTrigger<DealDamageAction>(
                (dda) => dda.Amount > 0 && dda.Target == CharacterCard && !AlreadyUsed(),
                RedirectDamage,
                TriggerType.WouldBeDealtDamage,
                TriggerTiming.Before
            );
        }

        private bool AlreadyUsed()
        {
            var props = Journal.CardPropertiesEntriesThisRound(cp => cp.Card == Card && cp.Key == "SimurghDamagedThisRound");
            return props.Where(cp => cp.BoolValue.HasValue && cp.BoolValue.Value).Count() > 0;
        }

        private IEnumerator RedirectDamage(DealDamageAction dda)
        {
            SetCardPropertyToTrueIfRealAction("SimurghDamagedThisRound");
            return RedirectDamage(
                dda,
                TargetType.LowestHP,
                c => c.Is().Hero().Character().Target()
            );
        }
    }
}
