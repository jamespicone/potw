using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.MissMilitia
{
    public class MissMilitiaCharacterCardController : MissMilitiaUtilityCharacterCardController
    {
        public MissMilitiaCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Deck, WeaponCard()).Condition = () => ! Card.IsFlipped;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int numCards = GetPowerNumeral(0, 2);

            // "Reveal the top 2 cards of your deck. Put any revealed Weapon cards into play and discard the rest."
            var e = RevealCards_PutSomeIntoPlay_DiscardRemaining(
                TurnTakerController, 
                TurnTaker.Deck,
                numCards,
                WeaponCard()
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            // "You may use a power on a Weapon card now."
            e = GameController.SelectAndUsePower(
                HeroTurnTakerController, 
                optional: true,
                (p) => p.CardController.Card.DoKeywordsContain("weapon"),
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

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator e;
            switch (index)
            {
                case 0: e = UseIncapOption1(); break;
                case 1: e = UseIncapOption2(); break;
                case 2: e = UseIncapOption3(); break;
                default: yield break;
            }

            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        private IEnumerator UseIncapOption1()
        {
            // "One hero target deals 1 target 2 projectile damage."
            var chooseHeroTarget = new List<SelectCardDecision>();
            var e = GameController.SelectCardAndStoreResults(
                HeroTurnTakerController,
                SelectionType.CardToDealDamage,
                new LinqCardCriteria((c) => c.IsInPlay && this.HasAlignment(c, CardAlignment.Hero, CardTarget.Target), "hero target", useCardsSuffix: false),
                chooseHeroTarget,
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

            var choice = chooseHeroTarget.FirstOrDefault();
            if (choice?.SelectedCard != null)
            {
                e = GameController.SelectTargetsAndDealDamage(
                    DecisionMaker,
                    new DamageSource(GameController, choice.SelectedCard),
                    amount: 2,
                    DamageType.Projectile,
                    numberOfTargets: 1,
                    optional: false,
                    requiredTargets: 1,
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
        }

        private IEnumerator UseIncapOption2()
        {
            // "One hero destroys one of their Equipment cards. If they do, they deal 1 target 4 projectile damage."
            var storedResults = new List<DestroyCardAction>();
            var e = GameController.SelectHeroToDestroyTheirCard(
                DecisionMaker,
                new LinqCardCriteria((c) => IsEquipment(c), "equipment"),
                optionalSelectHero: false,
                optionalDestroyCard: false,
                storedResults, 
                responsibleCard: Card,
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

            if (DidDestroyCard(storedResults))
            {
                var owner = storedResults.First().CardToDestroy.Card.Owner;
                var storedCharacter = new List<Card>();
                e = FindCharacterCard(owner, SelectionType.HeroToDealDamage, storedCharacter);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }

                var character = storedCharacter.FirstOrDefault();
                if (character != null)
                {
                    e = GameController.SelectTargetsAndDealDamage(
                        FindHeroTurnTakerController(character.Owner.ToHero()),
                        new DamageSource(GameController, character),
                        amount: 4, DamageType.Projectile,
                        numberOfTargets: 1,
                        optional: false,
                        requiredTargets: 1,
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
            }
        }

        private IEnumerator UseIncapOption3()
        {
            // "One player may play a card."
            var e = GameController.SelectHeroToPlayCard(HeroTurnTakerController, cardSource: GetCardSource());
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
