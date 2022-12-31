using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Skitter
{
    public class ImprovisationalIngenuityCardController : CardController
    {
        public ImprovisationalIngenuityCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            // "The first time each turn {SkitterCharacter} or the swarm would deal damage but that damage is prevented, you may use a power."
            AddTrigger<DealDamageAction>(
                dda => (this.IsSwarmDamage(dda) || dda.DamageSource.Card == CharacterCard) && ! dda.DidDealDamage && ! HasBeenSetToTrueThisTurn("DamagePreventedSoPowerUse") && ! dda.IsPretend,
                UseAPower,
                TriggerType.UsePower,
                TriggerTiming.After
            );
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // "Reveal the top 3 cards of your deck. Put 1 into your hand and shuffle the others back into your deck."
            return RevealCards_SelectSome_MoveThem_ReturnTheRest(
                HeroTurnTakerController,
                TurnTakerController,
                TurnTaker.Deck,
                c => true,
                numberOfMatchesToReveal: GetPowerNumeral(0, 3),
                numberOfRevealedCardsToChoose: GetPowerNumeral(1, 1),
                canPutInHand: true,
                canPlayCard: false,
                isPutIntoPlay: false,
                cardCriteriaDescription: "card"
            );
        }

        private IEnumerator UseAPower(DealDamageAction dda)
        {
            SetCardPropertyToTrueIfRealAction("DamagePreventedSoPowerUse", gameAction: dda);
            return GameController.SelectAndUsePower(HeroTurnTakerController, showMessage: true, cardSource: GetCardSource());
        }
    }
}
