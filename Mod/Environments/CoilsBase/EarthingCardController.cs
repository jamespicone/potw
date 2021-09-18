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
    public class EarthingCardController : CardController
    {
        public EarthingCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // "Reduce all cold and lightning damage by 2."
            AddReduceDamageTrigger(dda => dda.DamageType == DamageType.Cold || dda.DamageType == DamageType.Lightning, dda => 2);

            // "After damage that was reduced by this card is dealt or reduced to 0 this card deals every other Structure 1 irreducible energy damage."
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
            var e = GameController.DealDamage(
                DecisionMaker,
                Card,
                c => c.DoKeywordsContain("structure") && c != Card,
                amount: 1,
                DamageType.Energy,
                isIrreducible: true,
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
