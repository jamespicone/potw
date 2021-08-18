using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.BrocktonBay
{
    public class LeetCardController : DuoCardController
    {
        public LeetCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNonEnvironmentTargetWithLowestHP(1, 1);
        }

        public override void AddTriggers()
        {
            // "At the end of the environment turn, this card deals the non-environment target with the lowest HP 2 lightning damage."
            AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => !c.IsEnvironmentTarget, TargetType.LowestHP, 2, DamageType.Lightning, highestLowestRanking: 1, numberOfTargets: 1);
            // "If Über is in play, increase damage dealt by Über and Leet by 1."
            AddIncreaseDamageTrigger((DealDamageAction dda) => base.TurnTaker.FindCard(UberIdentifier).IsInPlayAndHasGameText && (dda.DamageSource.IsSameCard(base.Card) || dda.DamageSource.Card == base.TurnTaker.FindCard(UberIdentifier)), (DealDamageAction dda) => 1);
            base.AddTriggers();
        }
    }
}
