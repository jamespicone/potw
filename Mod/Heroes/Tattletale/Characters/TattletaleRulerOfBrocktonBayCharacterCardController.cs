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
    public class TattletaleRulerOfBrocktonBayCharacterCardController : HeroCharacterCardController
    {
        public TattletaleRulerOfBrocktonBayCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override IEnumerator UsePower(int index = 0)
        {
            // "You may use a power. You may use a power."
            IEnumerator powerCoroutine = base.GameController.SelectAndUsePower(base.HeroTurnTakerController, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(powerCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(powerCoroutine);
            }
            powerCoroutine = base.GameController.SelectAndUsePower(base.HeroTurnTakerController, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(powerCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(powerCoroutine);
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
