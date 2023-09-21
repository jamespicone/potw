using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Battery
{

    public static class BatteryExtensions
    {
        public static IEnumerator ChargeCard(this CardController source, Card toCharge)
        {
            var effect = new BatteryChargedStatusEffect(source.CardWithoutReplacements, "", source.Card, toCharge);
            var e = source.GameController.AddStatusEffect(effect, showMessage: true, cardSource: source.GetCardSource());
            if (source.UseUnityCoroutines) { yield return source.GameController.StartCoroutine(e); }
            else { source.GameController.ExhaustCoroutine(e); }
        }

        public static IEnumerator DischargeCard(this CardController source, Card toDischarge)
        {
            var effect = source.FindChargeStatusEffectFor(toDischarge);
            if (effect == null) yield break;

            var e = source.GameController.ExpireStatusEffect(effect, source.GetCardSource());
            if (source.UseUnityCoroutines) { yield return source.GameController.StartCoroutine(e); }
            else { source.GameController.ExhaustCoroutine(e); }
        }

        public static bool IsCharged(this CardController source, Card c)
        {
            var effect = source.FindChargeStatusEffectFor(c);
            return effect != null;
        }

        public static StatusEffect FindChargeStatusEffectFor(this CardController source, Card card)
        {
            foreach (var sc in source.GameController.StatusEffectControllers)
            {
                var bsc = sc.StatusEffect as BatteryChargedStatusEffect;
                if (bsc != null && bsc.ChargedCard == card) return bsc;
            }

            return null;
        }

        public static void ShowChargedStatus(this CardController source, SpecialStringMaker maker, Card card)
        {
            maker.ShowIfElseSpecialString(
                () => source.IsCharged(card),
                () => $"{card.Title} is {{Charged}}",
                () => $"{card.Title} is {{not Charged}}"
            );
        }

        public static bool IsDischargePower(this GameController gc, UsePowerAction action, HeroTurnTaker expectedUser)
        {
            if (action.HeroUsingPower.HeroTurnTaker != expectedUser) return false;

            var powerBaseCard = gc.FindCardController(action.Power.CardController.CardWithoutReplacements);
            var batteryBase = powerBaseCard as BatteryUtilityCharacterCardController;
            if (batteryBase == null) return false;

            return batteryBase.DischargePowerIndex == action.Power.Index;
        }

        public static bool IsDischargePower(this GameController gc, UsePowerJournalEntry entry, HeroTurnTaker expectedUser)
        {
            if (entry.PowerUser != expectedUser) { return false; }

            var powerBaseCard = gc.FindCardController(entry.CardWithPower);
            var batteryBase = powerBaseCard as BatteryUtilityCharacterCardController;
            if (batteryBase == null) return false;

            return batteryBase.DischargePowerIndex == entry.PowerIndex;
        }


    }

}