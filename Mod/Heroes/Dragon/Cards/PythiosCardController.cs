using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Dragon
{
    public class PythiosCardController : MechCardController
    {
        public PythiosCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowListOfCardsInPlay(new LinqCardCriteria((c) => c.IsTarget && c.HitPoints.HasValue && c.HitPoints.Value <= 3, "targets with 3 or less HP", false, false, "target with 3 or less HP", "targets with 3 or less HP"));
        }

        protected override int FocusCost() { return 1; }

        protected override void AddExtraTriggers()
        {
            // At the end of your turn this card deals 1 lightning damage to all targets with 3 or less HP
            AddDealDamageAtEndOfTurnTrigger(
                TurnTaker,
                Card,
                c => c.IsTarget && c.HitPoints.HasValue && c.HitPoints.Value <= 3,
                TargetType.All,
                amount: 1,
                DamageType.Lightning
            );
        }

        public override IEnumerator ActivateAbilityEx(CardDefinition.ActivatableAbilityDefinition definition)
        {
            if (definition.Name != "focus") { yield break; }

            switch (definition.Number)
            {
                default: yield break;

                case 1:
                    yield return BounceCard();
                    break;

                case 2:
                    yield return FindCard();
                    break;
            }
        }

        private IEnumerator BounceCard()
        {
            // Select a non-character-card target in play and put it on top of its deck.
            var stored = new List<SelectCardDecision>();
            var e = GameController.SelectCardAndStoreResults(
                HeroTurnTakerController,
                SelectionType.ReturnToDeck,
                new LinqCardCriteria(c => c.IsTarget && c.IsInPlay && !c.IsCharacter),
                stored,
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

            var selected = GetSelectedCard(stored);
            if (selected == null) { yield break; }

            e = GameController.MoveCard(
                TurnTakerController,
                selected,
                selected.NativeDeck,
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

        private IEnumerator FindCard()
        {
            // Reveal cards from the top of a deck until you reveal a target. You may either discard it or put it into play. Shuffle the other cards back into the deck.
            var stored = new List<SelectLocationDecision>();
            var e = GameController.SelectADeck(
                HeroTurnTakerController,
                SelectionType.RevealCardsFromDeck,
                l => true,
                stored,
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

            var selected = GetSelectedLocation(stored);
            if (selected == null) { yield break; }

            var revealed = new List<RevealCardsAction>();
            e = GameController.RevealCards(
                TurnTakerController,
                selected,
                c => c.IsTarget,
                numberOfMatches: 1,
                revealed,
                RevealedCardDisplay.ShowRevealedCards,
                GetCardSource()
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            var revealedCard = GetRevealedCard(revealed, false);
            if (revealedCard != null)
            {
                e = GameController.SelectLocationAndMoveCard(
                    HeroTurnTakerController,
                    revealedCard,
                    new MoveCardDestination[] {
                        new MoveCardDestination(revealedCard.Owner.Trash),
                        new MoveCardDestination(revealedCard.Owner.PlayArea)
                    },
                    isPutIntoPlay: true,
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

            e = CleanupRevealedCards(selected.OwnerTurnTaker.Revealed, selected, shuffleAfterwards: true);
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
