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
    public class LightningBoltCardController : BehemothUtilityCardController
    {
        public LightningBoltCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            // "Change {BehemothCharacter}'s damage type to lightning."
            IEnumerator lightningCoroutine = SetBehemothDamageType(base.Card, DamageType.Lightning);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(lightningCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(lightningCoroutine);
            }
            // "{BehemothCharacter} deals 1 hero 5 damage."
            List<SelectCardDecision> cardChoice = new List<SelectCardDecision>();
            IEnumerator damageCoroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 5, GetBehemothDamageType(), 1, false, 1, additionalCriteria: (Card c) => c.IsHeroCharacterCard, storedResultsDecisions: cardChoice, selectTargetsEvenIfCannotDealDamage: true, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(damageCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(damageCoroutine);
            }
            // "That hero's player may move 1 proximity token from their hero to another active hero."
            if (DidSelectCard(cardChoice))
            {
                TurnTaker passingTT = cardChoice.FirstOrDefault().SelectedCard.Owner;
                IEnumerator passCoroutine = MayMoveOneTokenResponse(passingTT);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(passCoroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(passCoroutine);
                }
            }
            yield break;
        }
    }
}
