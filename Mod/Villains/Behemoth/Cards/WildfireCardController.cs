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
    public class WildfireCardController : BehemothUtilityCardController
    {
        public WildfireCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            // "Change {BehemothCharacter}'s damage type to fire."
            IEnumerator fireCoroutine = SetBehemothDamageType(base.Card, DamageType.Fire);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(fireCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(fireCoroutine);
            }
            // "{BehemothCharacter} deals each hero with 3 or more proximity tokens {H} damage."
            IEnumerable<HeroTurnTakerController> heroControllers = FindActiveHeroTurnTakerControllers();
            List<Card> targets = new List<Card>();
            foreach (HeroTurnTakerController player in heroControllers)
            {
                TokenPool playerProximity = ProximityPool(player.TurnTaker);
                if (playerProximity.CurrentValue >= 3)
                {
                    if (player.HasMultipleCharacterCards)
                    {
                        List<Card> charResults = new List<Card>();
                        IEnumerator findCoroutine = FindCharacterCardToTakeDamage(player.TurnTaker, charResults, base.Card, H, GetBehemothDamageType());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(findCoroutine);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(findCoroutine);
                        }
                        targets.AddRange(charResults);
                    }
                    else
                    {
                        targets.Add(player.CharacterCard);
                    }
                }
            }
            IEnumerator damageCharactersCoroutine = base.GameController.DealDamage(DecisionMaker, base.CharacterCard, (Card c) => targets.Contains(c), H, GetBehemothDamageType(), cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(damageCharactersCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(damageCharactersCoroutine);
            }
            // "{BehemothCharacter} deals each non-character card hero target {H} damage."
            IEnumerator damageNonCharacterCoroutine = base.GameController.DealDamage(DecisionMaker, base.CharacterCard, (Card c) => c.IsHero && !c.IsCharacter, H, GetBehemothDamageType(), cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(damageNonCharacterCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(damageNonCharacterCoroutine);
            }
            yield break;
        }
    }
}
