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
    public class MissMilitiaProtectorateCaptainCharacterCardController : MissMilitiaUtilityCharacterCardController
    {
        public MissMilitiaProtectorateCaptainCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            AddThisCardControllerToList(CardControllerListType.ActivatesEffects);
            SpecialStringMaker.ShowListOfCardsInPlay(WeaponCard()).Condition = () => !base.Card.IsFlipped;
            SpecialStringMaker.ShowListOfCardsAtLocation(base.HeroTurnTaker.Hand, WeaponCard()).Condition = () => !base.Card.IsFlipped;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // "You may use a power on a Weapon card, activating all of its {sniper}{machete}{smg}{pistol} effects."
            List<SelectCardDecision> chosen = new List<SelectCardDecision>();
            IEnumerator selectWeaponCoroutine = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.UsePower, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("weapon"), "Weapon"), chosen, true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(selectWeaponCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(selectWeaponCoroutine);
            }
            Card selectedWeapon = GetSelectedCard(chosen);
            List<UsePowerDecision> results = new List<UsePowerDecision>();
            if (selectedWeapon != null)
            {
                CardController selectedController = FindCardController(selectedWeapon);
                selectedController.SetCardPropertyToTrueIfRealAction(WeaponCardController.ActivateAllIcons);
                IEnumerator powerCoroutine = base.GameController.SelectAndUsePower(base.HeroTurnTakerController, true, (Power p) => p.CardController == selectedController, storedResults: results, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(powerCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(powerCoroutine);
                }
                if (IsRealAction())
                {
                    selectedController.SetCardProperty(WeaponCardController.ActivateAllIcons, false);
                }
            }
            // "Return that card to your hand."
            if (WasPowerUsed(results))
            {
                IEnumerator moveCoroutine = base.GameController.MoveCard(base.TurnTakerController, selectedWeapon, base.HeroTurnTaker.Hand, responsibleTurnTaker: base.TurnTaker, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(moveCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(moveCoroutine);
                }
            }
            // "You may play a Weapon card."
            IEnumerator playCoroutine = base.GameController.SelectAndPlayCardFromHand(base.HeroTurnTakerController, true, cardCriteria: WeaponCard(), cardSource: GetCardSource());
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
            // "One hero deals 1 target 2 melee damage."
            List<SelectCardDecision> chooseHeroTarget = new List<SelectCardDecision>();
            IEnumerator chooseHeroCoroutine = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.CardToDealDamage, new LinqCardCriteria((Card c) => c.IsInPlay && c.IsHeroCharacterCard, "hero", useCardsSuffix: false, singular: "hero", plural: "heroes"), chooseHeroTarget, false, cardSource: GetCardSource());
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
                IEnumerator damageCoroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, choice.SelectedCard), 2, DamageType.Melee, 1, false, 1, cardSource: GetCardSource());
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

        private IEnumerator UseIncapOption3()
        {
            // "One hero regains 2 HP."
            IEnumerator healCoroutine = base.GameController.SelectAndGainHP(base.HeroTurnTakerController, 2, false, (Card c) => c.IsHeroCharacterCard, 1, 1, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(healCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(healCoroutine);
            }
            yield break;
        }
    }
}
