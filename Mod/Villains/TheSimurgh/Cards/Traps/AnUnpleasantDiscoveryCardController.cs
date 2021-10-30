using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.ParahumansOfTheWormverse.Utility;

using UnityEngine;

namespace Jp.ParahumansOfTheWormverse.TheSimurgh
{
    public class AnUnpleasantDiscoveryCardController : CardController
    {
        public AnUnpleasantDiscoveryCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            return card == Card;
        }

        public override void AddTriggers()
        {
            // "Whenever a hero card is discarded, that hero deals themself 1 psychic damage."
            AddTrigger<MoveCardAction>(
                mca => mca.IsDiscard && mca.CardToMove.IsHero && mca.CardToMove.Owner.IsHero,
                mca => HurtHero(mca),
                TriggerType.DealDamage,
                TriggerTiming.After
            );

            // "At the start of the villain turn...
            AddStartOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => DiscardHeroCards(pca),
                new TriggerType[] { TriggerType.DiscardCard, TriggerType.RevealCard }
            );
        }

        private IEnumerator HurtHero(MoveCardAction mca)
        {
            Debug.Log($"SimurghTrap: Hurting hero because {mca}");
            var heroToHurtTurnTaker = mca.CardToMove.Owner;

            var storedResults = new List<Card>();
            var damagePrototype = new DealDamageAction(GetCardSource(), null, null, 1, DamageType.Psychic);
            var e = GameController.FindCharacterCard(
                DecisionMaker,
                heroToHurtTurnTaker,
                SelectionType.DealDamageSelf,
                storedResults,
                damageInfo: new DealDamageAction[] { damagePrototype },
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

            var selected = storedResults.FirstOrDefault();
            if (selected == null) { yield break; }

            e = DealDamage(selected, selected, 1, DamageType.Psychic, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        private IEnumerator DiscardHeroCards(PhaseChangeAction pca)
        {
            // name a keyword.
            var heroCardsInDecks = FindCardsWhere(c => c.IsInDeck && c.Location.IsHero, ignoreBattleZone: true);
            var keywords = new HashSet<string>();

            foreach (Card c in heroCardsInDecks)
            {
                keywords.UnionWith(c.GetKeywords(true, true));
            }

            var storedResults = new List<SelectWordDecision>();
            var e = GameController.SelectWord(
                DecisionMaker,
                keywords.AsEnumerable(),
                SelectionType.SelectKeyword,
                storedResults,
                optional: true,
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

            var selectedKeyword = GetSelectedWord(storedResults);

            // Reveal the top 2 cards of each hero deck. Replace each card with the named keyword and discard the rest.",
            e = GameController.SelectLocationsAndDoAction(
                DecisionMaker,
                SelectionType.RevealCardsFromDeck,
                l => l.IsDeck && l.IsHero,
                l => RevealAndDiscard(l, selectedKeyword),
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

        private IEnumerator RevealAndDiscard(Location deck, string selectedKeyword)
        {
            var reveal = new RevealCardsAction(
                GetCardSource(),
                TurnTakerController,
                deck,
                numberOfCards: 2,
                fromBottom: false,
                revealedCardsDisplay: RevealedCardDisplay.ShowRevealedCards
            );

            var e = DoAction(reveal);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            var revealedLocation = reveal.RevealedCards.Select(c => c.Location).FirstOrDefault();
            if (revealedLocation == null) { yield break; }

            // Matching cards are cards that have one of the selected keywords
            e = GameController.BulkMoveCards(
                TurnTakerController,
                reveal.RevealedCards.Where(c => c.DoKeywordsContain(selectedKeyword)),
                deck,
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

            // Non-matching cards 
            e = GameController.MoveCards(
                TurnTakerController,
                revealedLocation.Cards,
                FindTrashFromDeck(deck),
                isDiscard: true,
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

            // paranoia
            e = CleanupRevealedCards(revealedLocation, deck);
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
