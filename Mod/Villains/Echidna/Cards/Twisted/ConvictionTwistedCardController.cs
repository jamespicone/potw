using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Echidna
{
    public class ConvictionTwistedCardController : CardController
    {
        public ConvictionTwistedCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            // "At the start of each hero turn this card deals that hero 1 melee damage.",
            AddPhaseChangeTrigger(
                tt => tt.Is().Hero(),
                p => p == Phase.Start,
                pca => true,
                pca => HurtHero(pca),
                new TriggerType[] { TriggerType.DealDamage },
                TriggerTiming.After
            );

            // "This card is immune to melee damage."
            AddImmuneToDamageTrigger(dda => dda.Target == Card && dda.DamageType == DamageType.Melee);
        }

        private IEnumerator HurtHero(PhaseChangeAction pca)
        {
            if (! pca.ToPhase.TurnTaker.Is().Hero()) { yield break; }

            var targetList = new List<Card>();
            var damage = new List<DealDamageAction>();
            damage.Add(new DealDamageAction(
                    GetCardSource(),
                    new DamageSource(GameController, Card),
                    target: null,
                    amount: 1,
                    DamageType.Melee
            ));

            var e = GameController.FindCharacterCard(
                DecisionMaker,
                pca.ToPhase.TurnTaker,
                SelectionType.DealDamage,
                targetList,
                cardSource: GetCardSource(),
                damageInfo: damage
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            var target = targetList.FirstOrDefault();
            if (target == null) { yield break; }

            e = DealDamage(
                Card,
                target,
                1,
                DamageType.Melee,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }
    }
}
