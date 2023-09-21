using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Battery
{
    public class CoolToysCardController : CardController
    {
        public CoolToysCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.ShowChargedStatus(SpecialStringMaker, CharacterCard);
            SpecialStringMaker.ShowNumberOfCardsInPlay(EquipmentOrDevice());
        }

        public override IEnumerator Play()
        {
            // If {BatteryCharacter} is {Charged}, she deals 1 target X lightning damage,
            // where X is the total number of Equipment and Device cards in play.
            if (this.IsCharged(CharacterCard))
            {
                var e = GameController.SelectTargetsAndDealDamage(
                    HeroTurnTakerController,
                    new DamageSource(GameController, CharacterCard),
                    FindCardsWhere(EquipmentOrDevice(), GetCardSource()).Count(),
                    DamageType.Lightning,
                    numberOfTargets: 1,
                    optional: false,
                    requiredTargets: 1,
                    cardSource: GetCardSource()
                );
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }

            // {Discharge} {BatteryCharacter}.
            {
                var e = this.DischargeCard(CharacterCard);
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }
        }

        private LinqCardCriteria EquipmentOrDevice()
        {
            return new LinqCardCriteria(
                c => c.IsInPlayAndHasGameText && (c.DoKeywordsContain("equipment") || c.DoKeywordsContain("device")),
                "Equipment or Device"
            );
        }
    }
}
