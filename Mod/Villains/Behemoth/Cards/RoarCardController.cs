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
    public class RoarCardController : BehemothUtilityCardController
    {
        public RoarCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            // "Change {BehemothCharacter}'s damage type to Sonic."
            IEnumerator sonicCoroutine = SetBehemothDamageType(base.Card, DamageType.Sonic);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(sonicCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(sonicCoroutine);
            }
            // "{BehemothCharacter} deals each non-villain target 2 damage."
            IEnumerator damageCoroutine = base.GameController.DealDamage(DecisionMaker, base.CharacterCard, (Card c) => !c.IsVillainTarget, 2, GetBehemothDamageType(), cardSource: GetCardSource());
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
