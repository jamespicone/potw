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
    public class AtlasCardController : CardController
    {
        public AtlasCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            // "{SkitterCharacter} is immune to damage from environment cards.",
            AddImmuneToDamageTrigger(dda => dda.Target == CharacterCard && dda.DamageSource.Is().Environment().Card());

            // "At the end of your turn draw a card"
            AddEndOfTurnTrigger(tt => tt == TurnTaker, pca => DrawCard(HeroTurnTaker), TriggerType.DrawCard);
        }
    }
}
