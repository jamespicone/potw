using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.MissMilitia
{
    public class MissMilitiaCharacterCardController : MissMilitiaUtilityCharacterCardController
    {
        public MissMilitiaCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(base.TurnTaker.Deck, WeaponCard()).Condition = () => !base.Card.IsFlipped;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int numCards = GetPowerNumeral(0, 2);
            // "Reveal the top 2 cards of your deck. Put any revealed Weapon cards into play and discard the rest."
            IEnumerator revealCoroutine = RevealCards_PutSomeIntoPlay_DiscardRemaining(base.TurnTakerController, base.TurnTaker.Deck, numCards, WeaponCard());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(revealCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(revealCoroutine);
            }
            // "You may use a power on a Weapon card now."
            IEnumerator powerCoroutine = base.GameController.SelectAndUsePower(base.HeroTurnTakerController, true, (Power p) => p.CardController.Card.DoKeywordsContain("weapon"), cardSource: GetCardSource());
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
            // "One hero target deals 1 target 2 projectile damage."
            List<SelectCardDecision> chooseHeroTarget = new List<SelectCardDecision>();
            IEnumerator chooseHeroCoroutine = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.CardToDealDamage, new LinqCardCriteria((Card c) => c.IsInPlay && c.IsTarget && c.IsHero, "hero target", useCardsSuffix: false), chooseHeroTarget, false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(chooseHeroCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(chooseHeroCoroutine);
            }
            SelectCardDecision choice = chooseHeroTarget.FirstOrDefault();
            if (choice != null && choice.SelectedCard != null)
            {
                IEnumerator damageCoroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, choice.SelectedCard), 2, DamageType.Projectile, 1, false, 1, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(damageCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(damageCoroutine);
                }
            }
            yield break;
        }

        private IEnumerator UseIncapOption2()
        {
            // "One hero destroys one of their Equipment cards. If they do, they deal 1 target 4 projectile damage."
            List<DestroyCardAction> storedResults = new List<DestroyCardAction>();
            IEnumerator destroyCoroutine = base.GameController.SelectHeroToDestroyTheirCard(DecisionMaker, new LinqCardCriteria((Card c) => IsEquipment(c), "equipment"), optionalSelectHero: false, optionalDestroyCard: false, storedResults, null, null, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(destroyCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(destroyCoroutine);
            }
            if (DidDestroyCard(storedResults))
            {
                TurnTaker owner = storedResults.First().CardToDestroy.Card.Owner;
                List<Card> storedCharacter = new List<Card>();
                IEnumerator findCoroutine = FindCharacterCard(owner, SelectionType.HeroToDealDamage, storedCharacter);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(findCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(findCoroutine);
                }
                Card character = storedCharacter.FirstOrDefault();
                if (character != null)
                {
                    IEnumerator damageCoroutine = base.GameController.SelectTargetsAndDealDamage(FindHeroTurnTakerController(character.Owner.ToHero()), new DamageSource(base.GameController, character), 4, DamageType.Projectile, 1, false, 1, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(damageCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(damageCoroutine);
                    }
                }
            }
            yield break;
        }

        private IEnumerator UseIncapOption3()
        {
            // "One player may play a card."
            IEnumerator playCoroutine = base.GameController.SelectHeroToPlayCard(base.HeroTurnTakerController, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(playCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(playCoroutine);
            }
            yield break;
        }
    }
}
