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
    public class DeadlySpidersCardController : CardController
    {
        public DeadlySpidersCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            //  "At the end of your turn the swarm deals up to 1 target X toxic damage, where X = 1 + the number of Bug tokens on this card."
            AddEndOfTurnTrigger(tt => tt == TurnTaker, pca => DealSpiderDamage(), TriggerType.DealDamage);

            this.AddResetTokenTrigger();
        }

        private IEnumerator DealSpiderDamage()
        {
            // "X on this card = 1 + the number of Bug tokens on this card or 3, whichever is lower.",
            // "At the end of your turn the swarm deals up to 1 target X toxic damage."
            return GameController.SelectTargetsAndDealDamage(
                HeroTurnTakerController,
                new DamageSource(GameController, FindEnvironment().TurnTaker),
                Math.Min(Card.BugTokenCount(), 3) + 1,
                DamageType.Toxic,
                numberOfTargets: 1,
                optional: false,
                requiredTargets: 0,
                cardSource: GetCardSource()
            );
        }
    }
}
