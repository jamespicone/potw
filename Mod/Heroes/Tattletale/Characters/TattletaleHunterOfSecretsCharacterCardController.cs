using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Tattletale
{
    public class TattletaleHunterOfSecretsCharacterCardController : HeroCharacterCardController
    {
        public TattletaleHunterOfSecretsCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override IEnumerator UsePower(int index = 0)
        {
            TokenPool infoPool = base.CharacterCard.FindTokenPool("TattletaleHunterOfSecretsPool");
            switch (index)
            {
                case 0:
                    // "Reveal the top card of your deck. ..."
                    List<Card> revealed = new List<Card>();
                    IEnumerator revealCoroutine = base.GameController.RevealCards(base.TurnTakerController, base.TurnTaker.Deck, 1, revealed, fromBottom: false, revealedCardDisplay: RevealedCardDisplay.Message, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(revealCoroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(revealCoroutine);
                    }
                    if (revealed.Count() > 0)
                    {
                        Card revealedCard = revealed.First();
                        if (!revealedCard.DoKeywordsContain("ongoing"))
                        {
                            // "... If it's not an Ongoing card, discard it."
                            IEnumerator discardCoroutine = base.GameController.MoveCard(base.TurnTakerController, revealedCard, base.TurnTaker.Trash, responsibleTurnTaker: base.TurnTaker, isDiscard: true, cardSource: GetCardSource());
                            if (UseUnityCoroutines)
                            {
                                yield return GameController.StartCoroutine(discardCoroutine);
                            }
                            else
                            {
                                GameController.ExhaustCoroutine(discardCoroutine);
                            }
                        }
                        else if (FindCardsWhere(new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.Title == revealedCard.Title), GetCardSource()).Count() <= 0)
                        {
                            // "If it's an Ongoing card and no other cards with that name are in play, put it into play..."
                            IEnumerator playCoroutine = base.GameController.PlayCard(base.TurnTakerController, revealedCard, isPutIntoPlay: true, responsibleTurnTaker: base.TurnTaker, cardSource: GetCardSource());
                            if (UseUnityCoroutines)
                            {
                                yield return GameController.StartCoroutine(playCoroutine);
                            }
                            else
                            {
                                GameController.ExhaustCoroutine(playCoroutine);
                            }
                            // "... and put a token on this card."
                            IEnumerator addCoroutine = base.GameController.AddTokensToPool(infoPool, 1, GetCardSource());
                            if (UseUnityCoroutines)
                            {
                                yield return GameController.StartCoroutine(addCoroutine);
                            }
                            else
                            {
                                GameController.ExhaustCoroutine(addCoroutine);
                            }
                        }
                        else
                        {
                            // "Otherwise, {TattletaleCharacter} deals herself X psychic damage..."
                            IEnumerator damageCoroutine = base.DealDamage(base.CharacterCard, base.CharacterCard, infoPool.CurrentValue, DamageType.Psychic, cardSource: GetCardSource());
                            if (UseUnityCoroutines)
                            {
                                yield return GameController.StartCoroutine(damageCoroutine);
                            }
                            else
                            {
                                GameController.ExhaustCoroutine(damageCoroutine);
                            }
                            // "... and remove all tokens from this card."
                            IEnumerator resetCoroutine = base.GameController.RemoveTokensFromPool(infoPool, infoPool.CurrentValue, cardSource: GetCardSource());
                            if (UseUnityCoroutines)
                            {
                                yield return GameController.StartCoroutine(resetCoroutine);
                            }
                            else
                            {
                                GameController.ExhaustCoroutine(resetCoroutine);
                            }
                        }
                    }
                    // Clean up revealed cards
                    List<Card> remaining = revealed.Where((Card c) => c.Location.IsRevealed).ToList();
                    if (remaining.Count() > 0)
                    {
                        List<Location> toClean = new List<Location>();
                        toClean.Add(base.TurnTaker.Revealed);
                        IEnumerator cleanCoroutine = CleanupCardsAtLocations(toClean, base.TurnTaker.Deck, cardsInList: remaining);
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(cleanCoroutine);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(cleanCoroutine);
                        }
                    }
                    break;
                case 1:
                    // "If there are any tokens on this card..."
                    if (infoPool.CurrentValue > 0)
                    {
                        // "... {TattletaleCharacter} may deal a target X psychic damage..."
                        IEnumerator damageCoroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), infoPool.CurrentValue, DamageType.Psychic, 1, true, 0, cardSource: GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(damageCoroutine);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(damageCoroutine);
                        }
                        // "... then remove a token from this card."
                        IEnumerator removeCoroutine = base.GameController.RemoveTokensFromPool(infoPool, 1, cardSource: GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(removeCoroutine);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(removeCoroutine);
                        }
                    }
                    break;
            }
            yield break;
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator incapCoroutine;
            switch (index)
            {
                case 0:
                    incapCoroutine = UseIncapOption1();
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(incapCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(incapCoroutine);
                    }
                    break;
                case 1:
                    incapCoroutine = UseIncapOption2();
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(incapCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(incapCoroutine);
                    }
                    break;
                case 2:
                    incapCoroutine = UseIncapOption3();
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(incapCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(incapCoroutine);
                    }
                    break;
            }
            yield break;
        }

        private IEnumerator UseIncapOption1()
        {
            // "One hero may use a power."
            IEnumerator powerCoroutine = base.GameController.SelectHeroToUsePower(base.HeroTurnTakerController, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(powerCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(powerCoroutine);
            }
            yield break;
        }

        private IEnumerator UseIncapOption2()
        {
            // "Reveal the top card of a deck, then replace it."
            List<SelectLocationDecision> chosen = new List<SelectLocationDecision>();
            IEnumerator chooseDeckCoroutine = base.GameController.SelectADeck(base.HeroTurnTakerController, SelectionType.RevealBottomCardOfDeck, (Location l) => l.HasCards, chosen, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(chooseDeckCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(chooseDeckCoroutine);
            }
            Location deck = GetSelectedLocation(chosen);
            if (deck != null)
            {
                List<Card> revealed = new List<Card>();
                IEnumerator revealCoroutine = base.GameController.RevealCards(base.TurnTakerController, deck, 1, revealed, revealedCardDisplay: RevealedCardDisplay.ShowRevealedCards, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(revealCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(revealCoroutine);
                }

                IEnumerator replaceCoroutine = base.CleanupRevealedCards(deck.OwnerTurnTaker.Revealed, deck);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(replaceCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(replaceCoroutine);
                }
            }
            yield break;
        }

        private IEnumerator UseIncapOption3()
        {
            // "Reveal the bottom card of a deck, then replace or discard it."
            List<SelectLocationDecision> chosen = new List<SelectLocationDecision>();
            IEnumerator chooseDeckCoroutine = base.GameController.SelectADeck(base.HeroTurnTakerController, SelectionType.RevealBottomCardOfDeck, (Location l) => l.HasCards, chosen, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(chooseDeckCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(chooseDeckCoroutine);
            }
            Location deck = GetSelectedLocation(chosen);
            if (deck != null)
            {
                IEnumerator decideCoroutine = base.RevealCard_DiscardItOrPutItOnDeck(base.HeroTurnTakerController, base.TurnTakerController, deck, true, true);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(decideCoroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(decideCoroutine);
                }
            }
            yield break;
        }
    }
}
