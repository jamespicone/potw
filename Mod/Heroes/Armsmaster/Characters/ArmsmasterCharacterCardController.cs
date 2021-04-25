using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Armsmaster
{
    public class ArmsmasterCharacterCardController : HeroCharacterCardController
    {
        public ArmsmasterCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(base.TurnTaker.Deck, new LinqCardCriteria((Card c) => c.DoKeywordsContain("equipment"), "equipment"));
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            /*
                "One target regains 2 HP",
                "Armsmaster deals 1 irreducible melee damage to a target",
                "One player may search their trash for an Equipment card and put it in their hand"
             */

            IEnumerator e;
            switch(index)
            {
                case 0:
                    e = GameController.SelectAndGainHP(
                        HeroTurnTakerController,
                        2,
                        cardSource: GetCardSource()
                    );
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(e);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(e);
                    };
                    break;

                case 1:
                    e = GameController.SelectTargetsAndDealDamage(
                        HeroTurnTakerController,
                        new DamageSource(GameController, CharacterCard),
                        1,
                        DamageType.Energy,
                        1,
                        optional: false,
                        1,
                        isIrreducible: true,
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

                    break;

                case 2:
                    e = GameController.SelectHeroToMoveCardFromTrash(
                        HeroTurnTakerController,
                        httc => httc.HeroTurnTaker.Hand,
                        cardCriteria: new LinqCardCriteria(c => c.DoKeywordsContain("equipment"), "Equipment"),
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

                    break;

                default: yield break;
            }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // Reveal the top card of your deck. If it's an Equipment card, put it in your hand; otherwise shuffle it back
            var e = RevealCards_MoveMatching_ReturnNonMatchingCards(
                TurnTakerController,
                TurnTaker.Deck,
                playMatchingCards: false,
                putMatchingCardsIntoPlay: false,
                moveMatchingCardsToHand: true,
                new LinqCardCriteria(c => c.DoKeywordsContain("equipment"), "Equipment"),
                numberOfMatches: null,
                numberOfCards: 1,
                shuffleSourceAfterwards: false,
                shuffleReturnedCards: true,
                revealedCardDisplay: RevealedCardDisplay.ShowRevealedCards
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
    }
}
