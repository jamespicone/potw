using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Coil
{
    public class DinahCardController : CardController
    {
        public DinahCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // "Scheming and Acting are immune to damage.",
            AddImmuneToDamageTrigger(
                dda => dda.Target.Identifier == "CoilSchemingCharacter" || dda.Target.Identifier == "CoilActingCharacter"
            );

            // "Whenever this card is dealt damage during a hero's turn that player discards a card"
            AddTrigger<DealDamageAction>(
                dda => dda.Target == Card && Game.ActiveTurnTaker.Is(this).Hero(),
                dda => HeroDiscards(dda),
                TriggerType.DiscardCard,
                TriggerTiming.After
            );
        }

        private IEnumerator HeroDiscards(DealDamageAction dda)
        {
            var htt = Game.ActiveTurnTaker as HeroTurnTaker;
            if (htt == null) { yield break; }

            var httc = FindHeroTurnTakerController(htt);
            if (httc == null) { yield break; }

            var e = GameController.SelectAndDiscardCard(
                httc,
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
