using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Battery
{
    public class BatteryCharacterCardController : BatteryUtilityCharacterCardController
    {
        public BatteryCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            DischargePowerIndex = 1;
            AddAsPowerContributor();
        }

        public override IEnumerable<Power> AskIfContributesPowersToCardController(CardController cardController)
        {
            if (cardController != this) { return null; }

            var ret = new List<Power>();

            if (IsBatteryCharged())
            {
                ret.Add(new Power(
                    cardController.HeroTurnTakerController,
                    cardController,
                    "{Discharge} {BatteryCharacter} and you may play a card.",
                    DischargePower(),
                    1,
                    null,
                    GetCardSource()
                ));
            }
            else
            {
                ret.Add(new Power(
                    cardController.HeroTurnTakerController,
                    cardController,
                    "{Charge} {BatteryCharacter} and draw a card.",
                    ChargePower(),
                    0,
                    null,
                    GetCardSource()
                ));
            }

            return ret;
        }

        private IEnumerator ChargePower()
        {
            IEnumerator chargeCoroutine = Charge(base.Card);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(chargeCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(chargeCoroutine);
            }
            IEnumerator drawCoroutine = base.GameController.DrawCard(base.HeroTurnTaker, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(drawCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(drawCoroutine);
            }
        }

        private IEnumerator DischargePower()
        {
            IEnumerator dischargeCoroutine = RemoveCharge(base.Card);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(dischargeCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(dischargeCoroutine);
            }
            IEnumerator playCoroutine = base.GameController.SelectAndPlayCardFromHand(base.HeroTurnTakerController, true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(playCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(playCoroutine);
            }
        }


        public override IEnumerator UsePower(int index = 0)
        {
            switch (index)
            {
                case 0:
                    // "If {BatteryCharacter} is {Discharged}, {Charge} {BatteryCharacter} and draw a card."
                    if (!IsBatteryCharged())
                    {
                        var e = ChargePower();
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(e);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(e);
                        }
                    }
                    else
                    {
                        IEnumerator messageCoroutine = base.GameController.SendMessageAction(base.Card.Title + " is already {Charged}, so her Charge power has no effect.", Priority.Medium, GetCardSource(), showCardSource: true);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(messageCoroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(messageCoroutine);
                        }
                    }
                    break;
                case 1:
                    // "If {BatteryCharacter} is {Charged}, {Discharge} {BatteryCharacter} and you may play a card."
                    if (IsBatteryCharged())
                    {
                        var e = DischargePower();
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(e);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(e);
                        }
                    }
                    else
                    {
                        IEnumerator messageCoroutine = base.GameController.SendMessageAction(base.Card.Title + " is already {Discharged}, so her Discharge power has no effect.", Priority.Medium, GetCardSource(), showCardSource: true);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(messageCoroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(messageCoroutine);
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
            // "One player may play a card now."
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

        private IEnumerator UseIncapOption2()
        {
            // "One hero target deals 1 target 2 lightning damage."
            List<SelectCardDecision> chooseHeroTarget = new List<SelectCardDecision>();
            IEnumerator chooseHeroCoroutine = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.CardToDealDamage, new LinqCardCriteria((Card c) => c.IsInPlay && c.Is().Hero().Target(), "hero target", useCardsSuffix: false), chooseHeroTarget, false, cardSource: GetCardSource());
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
                IEnumerator damageCoroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, choice.SelectedCard), 2, DamageType.Lightning, 1, false, 1, cardSource: GetCardSource());
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
            // "Put an Equipment card from a player's trash into their hand."
            IEnumerator playerMoveCoroutine = base.GameController.SelectHeroToMoveCardFromTrash(DecisionMaker, (HeroTurnTakerController httc) => httc.HeroTurnTaker.Hand, optionalSelectHero: false, optionalMoveCard: false, allowAutoDecide: false, toBottom: false, isPutIntoPlay: false, playIfMovingToPlayArea: true, null, new LinqCardCriteria((Card c) => IsEquipment(c), "equipment"), GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(playerMoveCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(playerMoveCoroutine);
            }
            yield break;
        }
    }
}
