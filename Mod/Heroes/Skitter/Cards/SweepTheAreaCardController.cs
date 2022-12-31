using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Skitter
{
    public class SweepTheAreaCardController : CardController
    {
        public SweepTheAreaCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator UsePower(int index = 0)
        {
            // Reveal the top 3 cards of a deck. Discard any revealed targets and return the remaining cards in any order.
            var deckList = new List<SelectLocationDecision>();
            var e = GameController.SelectADeck(
                HeroTurnTakerController,
                SelectionType.RevealCardsFromDeck,
                l => true,
                deckList,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            var selectedDeck = GetSelectedLocation(deckList);
            if (selectedDeck != null)
            {
                var revealedCards = new List<Card>();
                e = GameController.RevealCards(
                    TurnTakerController,
                    selectedDeck,
                    GetPowerNumeral(0, 3),
                    revealedCards,
                    revealedCardDisplay: RevealedCardDisplay.ShowRevealedCards,
                    cardSource: GetCardSource()
                );
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }

                var revealedTargets = revealedCards.Where(c => c.IsTarget).ToList();
                var revealedNontargets = revealedCards.Where(c => ! c.IsTarget).ToList();

                var discardOrder = new SelectCardsDecision(
                    GameController,
                    HeroTurnTakerController,
                    c => revealedTargets.Contains(c),
                    SelectionType.DiscardCard,
                    numberOfCards: revealedTargets.Count(),
                    eliminateOptions: true,
                    allowAutoDecide: true,
                    allAtOnce: false,
                    cardSource: GetCardSource()
                );

                e = GameController.SelectCardsAndDoAction(
                    discardOrder,
                    scd => GameController.DiscardCard(HeroTurnTakerController, scd.SelectedCard, null, responsibleTurnTaker: TurnTaker, cardSource: GetCardSource()),
                    cardSource: GetCardSource()
                );
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }

                e = GameController.SelectCardsFromLocationAndMoveThem(
                    HeroTurnTakerController,
                    selectedDeck.OwnerTurnTaker.Revealed,
                    revealedNontargets.Count(),
                    revealedNontargets.Count(),
                    new LinqCardCriteria(c => revealedNontargets.Contains(c)),
                    new MoveCardDestination[] { new MoveCardDestination(selectedDeck) },
                    cardSource: GetCardSource()
                );
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }

                e = CleanupCardsAtLocations(new List<Location> { selectedDeck.OwnerTurnTaker.Revealed }, selectedDeck, cardsInList: revealedCards);
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }

            // { SkitterCharacter} deals 1 psychic damage to all villain targets.
            e = DealDamage(
                CharacterCard,
                c => c.Is().Villain().Target().AccordingTo(this),
                GetPowerNumeral(1, 1),
                DamageType.Psychic
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }
    }
}
