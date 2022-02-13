using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.CoilsBase
{
    public class TrappedChamberCardController : CardController
    {
        public TrappedChamberCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // "Heroes cannot use powers."
            CannotUsePowers(ttc => ttc.TurnTaker.Is().Hero());

            // "At the start of the environment turn this card deals 2 projectile damage to the H nonenvironment targets with the highest HP"
            AddStartOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => ActivateTrap(),
                new TriggerType[] {
                    TriggerType.DealDamage,
                    TriggerType.DestroySelf
                }
            );
        }

        private IEnumerator ActivateTrap()
        {
            var e = DealDamageToHighestHP(
                Card,
                ranking: 1,
                targetCriteria: c => c.Is().NonEnvironment().Target(),
                c => 2,
                DamageType.Projectile,
                numberOfTargets: () => Game.H
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            // then destroys itself
            e = GameController.DestroyCard(
                null,
                Card,
                responsibleCard: Card,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }
    }
}
