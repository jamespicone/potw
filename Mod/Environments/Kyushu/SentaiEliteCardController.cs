using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.Kyushu
{
    public class SentaiEliteCardController : CardController
    {
        public SentaiEliteCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNonEnvironmentTargetWithHighestHP(ranking: 1, numberOfTargets: 1);
        }

        public override void AddTriggers()
        {
            // "At the end of the environment turn, this card deals the non-environment target with the highest HP X energy damage, where X is 2 times the number of Sentai Elite in play."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, AttackResponse, TriggerType.DealDamage);
            base.AddTriggers();
        }

        public IEnumerator AttackResponse(PhaseChangeAction pca)
        {
            // "... this card deals the non-environment target with the highest HP X energy damage, where X is 2 times the number of Sentai Elite in play."
            int x = FindCardsWhere(new LinqCardCriteria((Card c) => c.Identifier == base.Card.Identifier && c.IsInPlayAndHasGameText), GetCardSource()).Count() * 2;
            IEnumerator damageCoroutine = DealDamageToHighestHP(base.Card, 1, (Card c) => c.Is().NonEnvironment().Target(), (Card c) => x, DamageType.Energy);
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
