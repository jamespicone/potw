using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Grue
{
    public class SpewDarknessCardController : CardController
    {
        public SpewDarknessCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            // "Hero targets with a Darkness card next to them are immune to damage dealt by non-hero targets.",
            AddImmuneToDamageTrigger(
                dda => dda.DamageSource.Is(this).NonHero().Target() &&
                    dda.Target.Is(this).Hero().Target() &&
                    this.DoesTargetHaveDarknessAdjacent(dda.Target)
            );

            // "At the start of your turn destroy this card"
            AddStartOfTurnTrigger(tt => tt == TurnTaker, pca => DestroyThisCardResponse(pca), TriggerType.DestroySelf);
        }

        public override IEnumerator Play()
        {
            // "When this card enters play {GrueCharacter} deals himself 3 psychic damage;
            var e = DealDamage(CharacterCard, CharacterCard, 3, DamageType.Psychic, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            // then put a Darkness card into play next to a hero character card.",
            var selectedCardList = new List<SelectCardDecision>();
            e = GameController.SelectCardAndStoreResults(
                HeroTurnTakerController,
                SelectionType.MoveCardNextToCard,
                new LinqCardCriteria(c => c.Is(this).Hero().Character() && c.IsInPlay, "hero character"),
                storedResults: selectedCardList,
                optional: false,
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

            var selectedCard = GetSelectedCard(selectedCardList);
            if (selectedCard == null) { yield break; }

            e = this.PutDarknessIntoPlay(selectedCard);
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
