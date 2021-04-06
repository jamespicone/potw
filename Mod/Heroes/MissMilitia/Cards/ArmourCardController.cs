using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.MissMilitia
{
    public class ArmourCardController : MissMilitiaUtilityCardController
    {
        public ArmourCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // "If {MissMilitiaCharacter} would be dealt 5 or more damage from a single source, prevent that damage and destroy this card."
            AddPreventDamageTrigger((DealDamageAction dda) => dda.Target == base.CharacterCard && dda.Amount >= 5, (DealDamageAction dda) => base.GameController.DestroyCard(base.HeroTurnTakerController, base.Card, cardSource: GetCardSource()), new TriggerType[] { TriggerType.DestroySelf }, isPreventEffect: true);
            base.AddTriggers();
        }
    }
}
