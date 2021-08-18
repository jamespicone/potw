using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.Coil
{
    public class BallisticCardController : CardController
    {
        public BallisticCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // "Whenever a hero character is dealt damage by this card, destroy a noncharacter card belonging to that hero.",
            AddTrigger<DealDamageAction>(
                dda => dda.DamageSource.IsSameCard(Card) && dda.Target.IsHeroCharacterCard,
                dda => DestroyHeroCard(dda),
                TriggerType.DestroyCard,
                TriggerTiming.After
            );

            // "At the end of the villain turn this card deals {H + 1} projectile damage to the hero target with the highest HP"
            AddDealDamageAtEndOfTurnTrigger(
                TurnTaker,
                Card,
                c => c.IsHeroTarget(),
                TargetType.HighestHP,
                H + 1,
                DamageType.Projectile
            );
        }

        private IEnumerator DestroyHeroCard(DealDamageAction dda)
        {
            var herott = dda.Target.Owner as HeroTurnTaker;
            if (herott == null) { yield break; }

            var herottc = FindHeroTurnTakerController(herott);
            if (herottc == null) { yield break; }

            var e = GameController.SelectAndDestroyCard(
                herottc,
                new LinqCardCriteria(c => c.Owner == herott && !c.IsCharacter),
                optional: false,
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
