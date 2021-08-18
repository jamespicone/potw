using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Dragon
{
    public class SelfModifyingCodeCardController : CardController
    {
        public SelfModifyingCodeCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
        }

        public override IEnumerator Play()
        {
            // "Dragon deals herself 2 irreducible psychic damage",
            var results = new List<DealDamageAction>();
            var e = DealDamage(
                CharacterCard,
                CharacterCard,
                2,
                DamageType.Psychic,
                isIrreducible: true,
                storedResults: results,
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

            if (results.Count <= 0) { yield break; }
            var damageResult = results.First();
            if (damageResult == null) { yield break; }

            // "...If you take damage in this way, gain 3 focus points"
            if (! damageResult.DidDealDamage || damageResult.Target != CharacterCard)
            {
                yield break;
            }

            e = CharacterCardController.AddFocusTokens(3, GetCardSource());
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
