using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.CoilsBase
{
    public class AblativeCladdingCardController : CardController
    {
        public AblativeCladdingCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // "Reduce all toxic and melee damage by 2."
            AddReduceDamageTrigger((DealDamageAction dda) => dda.DamageType == DamageType.Melee || dda.DamageType == DamageType.Toxic, (DealDamageAction dda) => 2);

            // "After damage reduced by this card resolves this card deals itself 1 irreducible melee damage (even if damage wasn't dealt)."
            AddTrigger<DealDamageAction>(
                dda => WasReduced(dda),
                dda => Ablate(dda),
                TriggerType.DealDamage,
                TriggerTiming.After,
                requireActionSuccess: false
            );
        }

        private bool WasReduced(DealDamageAction dda)
        {
            return dda.DamageModifiers.Count(mdda => mdda.IsSuccessful && mdda.CardSource?.Card == Card) > 0;
        }

        private IEnumerator Ablate(DealDamageAction dda)
        {
            var e = DealDamage(Card, Card, 1, DamageType.Melee, isIrreducible: true, cardSource: GetCardSource());
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
