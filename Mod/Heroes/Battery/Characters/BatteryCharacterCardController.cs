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

            if (this.IsCharged(Card))
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
            var e = this.ChargeCard(CharacterCard);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            e = DrawCard(HeroTurnTaker);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }

        private IEnumerator DischargePower()
        {
            var e = this.DischargeCard(CharacterCard);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            e = SelectAndPlayCardFromHand(HeroTurnTakerController);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator e;
            switch (index)
            {
                // One player may play a card now.
                case 0: e = SelectHeroToPlayCard(HeroTurnTakerController); break;

                // Battery deals 1 target 2 lightning damage
                case 1:
                    e = GameController.SelectTargetsAndDealDamage(
                        HeroTurnTakerController,
                        new DamageSource(GameController, TurnTaker),
                        2,
                        DamageType.Lightning,
                        numberOfTargets: 1,
                        optional: false,
                        requiredTargets: 1,
                        cardSource: GetCardSource()
                    );
                    break;

                // Put an Equipment card from a player's trash into their hand.
                case 2:
                    e = GameController.SelectHeroToMoveCardFromTrash(
                        HeroTurnTakerController,
                        htt => htt.HeroTurnTaker.Hand,
                        optionalMoveCard: false,
                        cardCriteria: new LinqCardCriteria(c => IsEquipment(c), "equipment"),
                        cardSource: GetCardSource()
                    );
                    break;

                default: yield break;
            }

            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }
    }
}
