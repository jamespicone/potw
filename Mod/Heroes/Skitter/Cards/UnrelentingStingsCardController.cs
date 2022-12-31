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
    public class UnrelentingStingsCardController : CardController
    {
        public UnrelentingStingsCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            AddEndOfTurnTrigger(tt => tt == TurnTaker, pca => DoDamage(), TriggerType.DealDamage);

            this.AddResetTokenTrigger();
        }

        private IEnumerator DoDamage()
        {
            // At the end of your turn the swarm deals up to X targets 1 toxic damage, where X = 1 + the number of Bug tokens on this card."
            return GameController.SelectTargetsAndDealDamage(
                HeroTurnTakerController,
                new DamageSource(GameController, FindEnvironment().TurnTaker),
                1,
                DamageType.Toxic,
                numberOfTargets: 1 + Card.BugTokenCount(),
                optional: false,
                requiredTargets: 0,
                cardSource: GetCardSource()
            );
        }
    }
}
