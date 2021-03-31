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
    public class QuickInsightCardController : CardController
    {
        public QuickInsightCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // "Whenever a target enters play, you may reveal the bottom card of a deck. If you do, put the revealed card on the top or bottom of that deck and {TattletaleCharacter} deals herself 1 psychic damage."
            base.AddTrigger<CardEntersPlayAction>((CardEntersPlayAction cepa) => cepa.CardEnteringPlay != base.Card && cepa.CardEnteringPlay.IsTarget && GameController.IsCardVisibleToCardSource(cepa.CardEnteringPlay, GetCardSource()), PeekResponse, new TriggerType[] { TriggerType.RevealCard, TriggerType.MoveCard, TriggerType.DealDamage }, TriggerTiming.After);
            base.AddTriggers();
        }

        public IEnumerator PeekResponse(CardEntersPlayAction cepa)
        {
            // "... you may reveal the bottom card of a deck. If you do, put the revealed card on the top or bottom of that deck and {TattletaleCharacter} deals herself 1 psychic damage."
            // Choose deck to reveal from (or choose not to reveal)
            List<SelectLocationDecision> chosen = new List<SelectLocationDecision>();
            IEnumerator chooseDeckCoroutine = base.GameController.SelectADeck(base.HeroTurnTakerController, SelectionType.RevealBottomCardOfDeck, (Location l) => l.HasCards, chosen, optional: true, cardSource: GetCardSource());
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
                List<Card> results = new List<Card>();
                IEnumerator revealCoroutine = base.GameController.RevealCards(base.TurnTakerController, deck, 1, results, fromBottom: true, RevealedCardDisplay.None, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(revealCoroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(revealCoroutine);
                }
                Card revealedCard = results.FirstOrDefault();
                if (revealedCard != null)
                {
                    // Reveal the card and either move it to the top or bottom of its deck
                    List<MoveCardDestination> options = new List<MoveCardDestination>();
                    options.Add(new MoveCardDestination(deck));
                    options.Add(new MoveCardDestination(deck, toBottom: true));
                    IEnumerator moveCoroutine = base.GameController.SelectLocationAndMoveCard(base.HeroTurnTakerController, revealedCard, options, responsibleTurnTaker: base.TurnTaker, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(moveCoroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(moveCoroutine);
                    }
                    List<Location> toClean = new List<Location>();
                    toClean.Add(deck.OwnerTurnTaker.Revealed);
                    IEnumerator cleanCoroutine = CleanupCardsAtLocations(toClean, deck, cardsInList: results);
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(cleanCoroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(cleanCoroutine);
                    }
                    // Tattletale deals herself 1 psychic damage
                    IEnumerator damageCoroutine = base.DealDamage(base.CharacterCard, base.CharacterCard, 1, DamageType.Psychic, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(damageCoroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(damageCoroutine);
                    }
                }
            }
            yield break;
        }
    }
}
