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
    public class HelicopterCardController : TheMerchantsUtilityCardController
    {
        public HelicopterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // "{SkidmarkCharacter} is immune to melee damage."
            AddImmuneToDamageTrigger((DealDamageAction dda) => dda.Target == base.CharacterCard && dda.DamageType == DamageType.Melee);
            // "At the end of the villain turn, play the top card of the Thug deck."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => base.GameController.PlayTopCardOfLocation(base.TurnTakerController, ThugDeck, responsibleTurnTaker: base.TurnTaker, cardSource: GetCardSource(), showMessage: true), TriggerType.PlayCard);
            base.AddTriggers();
        }
    }
}
