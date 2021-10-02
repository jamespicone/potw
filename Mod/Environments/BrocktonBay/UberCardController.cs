using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.BrocktonBay
{
    public class UberCardController : DuoCardController
    {
        public UberCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNonEnvironmentTargetWithHighestHP(ranking: 1, numberOfTargets: 1);
        }

        public override void AddTriggers()
        {
            // "At the end of the environment turn, this card deals the non-environment target with the highest HP 2 melee damage. Then, if Leet is in play, Über and Leet each regain 1 HP."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, AttackHealResponse, new TriggerType[] { TriggerType.DealDamage, TriggerType.GainHP });
            base.AddTriggers();
        }

        public IEnumerator AttackHealResponse(PhaseChangeAction pca)
        {
            // "... this card deals the non-environment target with the highest HP 2 melee damage."
            IEnumerator damageCoroutine = DealDamageToHighestHP(base.Card, 1, (Card c) => c.Is().NonEnvironment().Target(), (Card c) => 2, DamageType.Melee);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(damageCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(damageCoroutine);
            }
            // "Then, if Leet is in play, Über and Leet each regain 1 HP."
            Card partner = base.TurnTaker.FindCard(LeetIdentifier);
            if (partner.IsInPlayAndHasGameText)
            {
                IEnumerator healCoroutine = base.GameController.GainHP(DecisionMaker, (Card c) => (c == base.Card || c == partner), 1, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(healCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(healCoroutine);
                }
            }
            yield break;
        }
    }
}
