using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Coil
{
    public class CoilCharacterCardController : VillainCharacterCardController
    {
        public CoilCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
        }

        public override void AddTriggers()
        {
            AddTrigger<GameAction>(
                ga => AllCoilsAreDead() && ((ga is FlipCardAction) || (ga is DestroyCardAction) || (ga is RemoveTargetAction) || (ga is MoveCardAction)),
                ga => DefeatedResponse(ga),
                TriggerType.GameOver,
                TriggerTiming.After
            );
        }

        private bool AllCoilsAreDead()
        {
            var acting = FindCard("CoilActingCharacter");
            var scheming = FindCard("CoilSchemingCharacter");

            var actingDead = acting == null || !acting.IsTarget || acting.IsOutOfGame || ! acting.IsInPlay;
            var schemingDead = scheming == null || !scheming.IsTarget || scheming.IsOutOfGame || ! scheming.IsInPlay;

            return actingDead && schemingDead;
        }
    }
}
