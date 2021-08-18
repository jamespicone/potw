using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.NewDelhi
{
    public class YangbanNewDelhiCardController : CardController
    {
        public YangbanNewDelhiCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHighestHP(ranking: 1, numberOfTargets: () => 1, cardCriteria: new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.IsTarget));
        }

        public override void AddTriggers()
        {
            // "At the end of the environment turn:{BR}{ This card deals the target with the highest HP 1 fire damage.{BR}{ This card deals the target with the highest HP 1 cold damage.{BR}{ This card deals the target with the highest HP 1 lightning damage.{BR}{ This card deals the target with the highest HP 1 energy damage."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DamageSequenceResponse, TriggerType.DealDamage);
            base.AddTriggers();
        }

        public IEnumerator DamageSequenceResponse(PhaseChangeAction pca)
        {
            // "... This card deals the target with the highest HP 1 fire damage."
            IEnumerator fireCoroutine = DealDamageToHighestHP(base.Card, 1, (Card c) => true, (Card c) => 1, DamageType.Fire);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(fireCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(fireCoroutine);
            }
            // "This card deals the target with the highest HP 1 cold damage."
            IEnumerator coldCoroutine = DealDamageToHighestHP(base.Card, 1, (Card c) => true, (Card c) => 1, DamageType.Cold);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coldCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coldCoroutine);
            }
            // "This card deals the target with the highest HP 1 lightning damage."
            IEnumerator lightningCoroutine = DealDamageToHighestHP(base.Card, 1, (Card c) => true, (Card c) => 1, DamageType.Lightning);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(lightningCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(lightningCoroutine);
            }
            // "This card deals the target with the highest HP 1 energy damage."
            IEnumerator energyCoroutine = DealDamageToHighestHP(base.Card, 1, (Card c) => true, (Card c) => 1, DamageType.Energy);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(energyCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(energyCoroutine);
            }
            yield break;
        }
    }
}
