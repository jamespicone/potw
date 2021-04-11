using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.BrocktonBay
{
    public class ScumCardController : CardController
    {
        public ScumCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNonEnvironmentTargetWithHighestHP(ranking: 2, numberOfTargets: 1);
        }

        public override void AddTriggers()
        {
            // "At the end of the environment turn, this card deals the non-environment target with the second highest HP X projectile damage, where X is 2 times the number of Scum in play."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, AttackResponse, TriggerType.DealDamage);
            base.AddTriggers();
        }

        public IEnumerator AttackResponse(PhaseChangeAction pca)
        {
            // "... this card deals the non-environment target with the second highest HP X projectile damage, where X is 2 times the number of Scum in play."
            int x = base.GameController.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("scum"), "scum", useCardsSuffix: false), visibleToCard: GetCardSource()).Count() * 2;
            IEnumerator damageCoroutine = DealDamageToHighestHP(base.Card, 2, (Card c) => !c.IsEnvironmentTarget, (Card c) => x, DamageType.Projectile);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(damageCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(damageCoroutine);
            }
            yield break;
        }
    }
}
