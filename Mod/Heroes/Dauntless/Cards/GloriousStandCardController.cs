using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Dauntless
{
    public class GloriousStandCardController : CardController
    {
        public GloriousStandCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            // "Whenever a hero target would be dealt damage, redirect the damage to {DauntlessCharacter}",
            AddRedirectDamageTrigger(
                dda => dda.Target.IsHero,
                () => CharacterCard
            );

            // "At the start of your turn destroy this card"
            AddStartOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => DestroyThisCardResponse(pca),
                TriggerType.DestroySelf
            );            
        }
    }
}
