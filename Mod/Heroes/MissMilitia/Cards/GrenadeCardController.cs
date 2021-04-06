using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.MissMilitia
{
    public class GrenadeCardController : MissMilitiaUtilityCardController
    {
        public GrenadeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            // "{MissMilitiaCharacter} deals a non-hero target 2 fire damage and 2 melee damage."
            List<DealDamageAction> storedResultsDamage = new List<DealDamageAction>();
            List<DealDamageAction> attacks = new List<DealDamageAction>();
            attacks.Add(new DealDamageAction(GetCardSource(), new DamageSource(base.GameController, base.CharacterCard), null, 2, DamageType.Fire));
            attacks.Add(new DealDamageAction(GetCardSource(), new DamageSource(base.GameController, base.CharacterCard), null, 2, DamageType.Melee));
            IEnumerator damageCoroutine = SelectTargetsAndDealMultipleInstancesOfDamage(attacks, (Card c) => !c.IsHero, null, 1, 1, false, storedResultsDamage);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(damageCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(damageCoroutine);
            }
            // "If she dealt damage this way, you may destroy an Ongoing card."
            if (DidDealDamage(storedResultsDamage))
            {
                IEnumerator destroyCoroutine = base.GameController.SelectAndDestroyCard(base.HeroTurnTakerController, new LinqCardCriteria((Card c) => c.DoKeywordsContain("ongoing"), "ongoing"), true, responsibleCard: base.Card, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(destroyCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(destroyCoroutine);
                }
            }
            yield break;
        }
    }
}
