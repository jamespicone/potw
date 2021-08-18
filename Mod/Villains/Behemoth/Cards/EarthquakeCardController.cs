using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Behemoth
{
    public class EarthquakeCardController : BehemothUtilityCardController
    {
        public EarthquakeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            // "Change {BehemothCharacter}'s damage type to melee."
            IEnumerator meleeCoroutine = SetBehemothDamageType(base.Card, DamageType.Melee);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(meleeCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(meleeCoroutine);
            }
            // "Destroy all environment cards."
            IEnumerator destroyEnvCoroutine = base.GameController.DestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => c.IsEnvironment, "environment"), cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(destroyEnvCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(destroyEnvCoroutine);
            }
            // "Destroy all Ongoing cards."
            IEnumerator destroyOngCoroutine = base.GameController.DestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => c.DoKeywordsContain("ongoing"), "ongoing"), cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(destroyOngCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(destroyOngCoroutine);
            }
            // "Destroy all Equipment cards."
            IEnumerator destroyEqpCoroutine = base.GameController.DestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => c.DoKeywordsContain("equipment"), "equipment"), cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(destroyEqpCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(destroyEqpCoroutine);
            }
            // "{BehemothCharacter} deals each hero target 2 damage."
            IEnumerator damageCoroutine = base.GameController.DealDamage(DecisionMaker, base.CharacterCard, (Card c) => c.IsHero, 2, GetBehemothDamageType(), cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(damageCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(damageCoroutine);
            }
            yield break;
        }
    }
}
