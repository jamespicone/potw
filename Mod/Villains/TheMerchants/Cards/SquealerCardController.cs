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
    public class SquealerCardController : TheMerchantsUtilityCardController
    {
        public SquealerCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            //AddThisCardControllerToList(CardControllerListType.ChangesVisibility);
        }

        public override void AddTriggers()
        {
            // "Environment cards ignore villain targets when dealing damage."
            // Approximating this with "Villain targets are immune to damage dealt by environment cards," because you can make a card invisible to another card, but I don't think you can choose whether it should be visible or not depending on what the other card is doing
            AddImmuneToDamageTrigger((DealDamageAction dda) => dda.DamageSource.IsEnvironmentSource && dda.Target.IsVillainTarget);
            // "At the start of the villain turn, play the top card of the environment deck."
            AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, PlayTheTopCardOfTheEnvironmentDeckResponse, TriggerType.PlayCard);
            base.AddTriggers();
        }
    }
}
