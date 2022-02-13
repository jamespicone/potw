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
    public class StaticChargeCardController : BatteryUtilityCardController
    {
        public StaticChargeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => "There has not been a previous turn for " + base.TurnTaker.Name).Condition = () => !HasBatteryStartedMoreThanOneTurnThisGame();
            SpecialStringMaker.ShowIfElseSpecialString(HasBatteryUsedDischargeSinceStartOfLastTurn, () => base.CharacterCard.Title + " has used her 'Discharge' power since the start of her last turn.", () => base.CharacterCard.Title + " has not used her 'Discharge' power since the start of her last turn.").Condition = () => HasBatteryStartedMoreThanOneTurnThisGame();
        }

        public override void AddTriggers()
        {
            // "At the start of your turn, if {BatteryCharacter} has used her 'Discharge' power since the start of your last turn, {BatteryCharacter} deals each non-hero target 2 lightning damage."
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker && HasBatteryUsedDischargeSinceStartOfLastTurn(), LightningDamageResponse, TriggerType.DealDamage);
            base.AddTriggers();
        }

        public IEnumerator LightningDamageResponse(PhaseChangeAction pca)
        {
            // "... {BatteryCharacter} deals each non-hero target 2 lightning damage."
            IEnumerator damageCoroutine = base.GameController.DealDamage(base.HeroTurnTakerController, base.CharacterCard, (Card c) => c.Is().NonHero().Target(), 2, DamageType.Lightning, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(damageCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(damageCoroutine);
            }
            yield break;
        }

        public bool HasBatteryStartedMoreThanOneTurnThisGame()
        {
            return (from pcje in Journal.PhaseChangeEntries() where pcje.ToPhase != null && pcje.ToPhase.TurnTaker == base.TurnTaker && pcje.ToPhase.IsStart select pcje).Count() > 1;
        }

        public bool HasBatteryUsedDischargeSinceStartOfLastTurn()
        {
            // If Battery has started her turn more than once this game...
            if (HasBatteryStartedMoreThanOneTurnThisGame())
            {
                // Find the last time Battery started her turn that wasn't the start of the current turn...
                PhaseChangeJournalEntry startOfThisTurn = Journal.PhaseChangeEntries().Where((PhaseChangeJournalEntry pcje) => pcje.ToPhase.IsStart).LastOrDefault();
                PhaseChangeJournalEntry startOfBatterysLastTurn = Journal.PhaseChangeEntries().Where((PhaseChangeJournalEntry pcje) => pcje.ToPhase.IsStart && pcje.ToPhase.TurnTaker == base.TurnTaker && pcje != startOfThisTurn).LastOrDefault();
                // Find all UsePowerJournalEntries that match IsBatteryUsingDischargePower and take place between startOfBatterysLastTurn and now
                IEnumerable<UsePowerJournalEntry> dischargesSinceStartOfLastTurn = Journal.QueryJournalEntries<UsePowerJournalEntry>((UsePowerJournalEntry upje) => Journal.GetEntryIndex(upje) > Journal.GetEntryIndex(startOfBatterysLastTurn) && IsBatteryUsingDischargePower(upje));
                if (dischargesSinceStartOfLastTurn != null && dischargesSinceStartOfLastTurn.Count() > 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
