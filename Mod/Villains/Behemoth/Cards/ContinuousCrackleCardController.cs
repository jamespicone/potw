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
    public class ContinuousCrackleCardController : BehemothUtilityCardController
    {
        public ContinuousCrackleCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // "Whenever a hero Ongoing or Equipment card enters play, {BehemothCharacter} deals the associated hero 2 damage."
            AddTrigger<CardEntersPlayAction>((CardEntersPlayAction cepa) => cepa.CardEnteringPlay.IsHero && (cepa.CardEnteringPlay.DoKeywordsContain("ongoing") || cepa.CardEnteringPlay.DoKeywordsContain("equipment")), DamageResponse, TriggerType.DealDamage, TriggerTiming.After);
            base.AddTriggers();
        }

        public override IEnumerator Play()
        {
            // "When this card enters play, change {BehemothCharacter}'s damage type to lightning."
            IEnumerator lightningCoroutine = SetBehemothDamageType(base.Card, DamageType.Lightning);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(lightningCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(lightningCoroutine);
            }
            yield break;
        }

        public IEnumerator DamageResponse(CardEntersPlayAction cepa)
        {
            // "... {BehemothCharacter} deals the associated hero 2 damage."
            List<Card> characterResults = new List<Card>();
            IEnumerator findCoroutine = FindCharacterCardToTakeDamage(cepa.CardEnteringPlay.Owner, characterResults, base.CharacterCard, 2, GetBehemothDamageType());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(findCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(findCoroutine);
            }
            Card target = characterResults.FirstOrDefault();
            if (target != null)
            {
                IEnumerator damageCoroutine = DealDamage(base.CharacterCard, target, 2, GetBehemothDamageType(), cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(damageCoroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(damageCoroutine);
                }
            }
            yield break;
        }
    }
}
