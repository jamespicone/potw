using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Slaughterhouse9
{
    public class WeDontTakePrisonersCardController : CardController
    {
        public WeDontTakePrisonersCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // Whenever a target is destroyed by a villain card each hero character deals themselves 2 psychic damage.
            AddTrigger<DestroyCardAction>(
                dca => dca.CardToDestroy.Card.IsTarget && (
                    dca.ResponsibleCard.Is(this).Villain() ||
                    ((dca.ActionSource is DealDamageAction) && (dca.ActionSource as DealDamageAction).DamageSource.Is(this).Villain())
                ),
                dca => HurtHeroes(),
                TriggerType.DealDamage,
                TriggerTiming.After
            );
        }

        private IEnumerator HurtHeroes()
        {
            var cards = FindCardsWhere(new LinqCardCriteria(c => c.Is(this).Hero().Target().Character() && c.IsInPlayAndHasGameText), GetCardSource());

            var e = GameController.SelectTargetsToDealDamageToSelf(
                DecisionMaker,
                2,
                DamageType.Psychic,
                cards.Count(),
                optional: false,
                allowAutoDecide: true,
                requiredTargets: null,
                additionalCriteria: c => c.Is(this).Hero().Target().Character(),
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
