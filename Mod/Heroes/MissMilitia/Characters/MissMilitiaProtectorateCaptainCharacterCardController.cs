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
            SpecialStringMaker.ShowListOfCardsInPlay(WeaponCard()).Condition = () => ! Card.IsFlipped;
            SpecialStringMaker.ShowListOfCardsAtLocation(HeroTurnTaker.Hand, WeaponCard()).Condition = () => ! Card.IsFlipped;
        }

        private bool activateAllWeaponEffects = false;
        public bool ConsumeActivateAllWeaponEffects()
        {
            var ret = activateAllWeaponEffects;
            activateAllWeaponEffects = false;
            return ret;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // "You may use a power on a Weapon card, activating all of its {sniper}{machete}{smg}{pistol} effects."
            var chosen = new List<SelectCardDecision>();
            var e = GameController.SelectCardAndStoreResults(
                HeroTurnTakerController,
                SelectionType.UsePower,
                new LinqCardCriteria((c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("weapon"), "Weapon"),
                chosen,
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

            var selectedWeapon = GetSelectedCard(chosen);
            if (selectedWeapon != null)
            {
                var results = new List<UsePowerDecision>();
                CardController selectedController = FindCardController(selectedWeapon);

                activateAllWeaponEffects = true;
                e = GameController.SelectAndUsePower(
                    HeroTurnTakerController,
                    powerCriteria: (p) => p.CardController == selectedController,
                    storedResults: results,
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
                activateAllWeaponEffects = false;

                // "Return that card to your hand."
                if (WasPowerUsed(results))
                {
                    e = GameController.MoveCard(TurnTakerController, selectedWeapon, HeroTurnTaker.Hand, responsibleTurnTaker: TurnTaker, cardSource: GetCardSource());
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
            
            // "You may play a Weapon card."
            e = GameController.SelectAndPlayCardFromHand(HeroTurnTakerController, optional: true, cardCriteria: WeaponCard(), cardSource: GetCardSource());
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
            // "One hero may use a power."
            var e = GameController.SelectHeroToUsePower(HeroTurnTakerController, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        private IEnumerator UseIncapOption2()
        {
            // "One hero deals 1 target 2 melee damage."
            var chooseHeroTarget = new List<SelectCardDecision>();
            var e = GameController.SelectCardAndStoreResults(
                HeroTurnTakerController,
                SelectionType.CardToDealDamage,
                new LinqCardCriteria((c) => c.IsInPlay && c.IsHeroCharacterCard, "hero", useCardsSuffix: false, singular: "hero", plural: "heroes"),
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
                    DamageType.Melee,
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

        private IEnumerator UseIncapOption3()
        {
            // "One hero regains 2 HP."
            var e = GameController.SelectAndGainHP(
                HeroTurnTakerController,
                amount: 2,
                additionalCriteria: (c) => c.IsHeroCharacterCard,
                requiredDecisions: 1, cardSource: GetCardSource()
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
