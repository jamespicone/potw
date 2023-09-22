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
    public class StaticChargeCardController : CardController
    {
        public StaticChargeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowIfElseSpecialString(
                HasBatteryUsedDischargeSinceStartOfLastTurn,
                () => CharacterCard.Title + " has used her 'Discharge' power since the start of her last turn.",
                () => CharacterCard.Title + " has not used her 'Discharge' power since the start of her last turn."
            );
        }

        public override void AddTriggers()
        {
            // At the start of your turn, if {BatteryCharacter} has used her 'Discharge' power since the start of your last turn...
            AddStartOfTurnTrigger(
                tt => tt == TurnTaker && HasBatteryUsedDischargeSinceStartOfLastTurn(),
                LightningDamageResponse,
                TriggerType.DealDamage
            );
        }

        public IEnumerator LightningDamageResponse(PhaseChangeAction pca)
        {
            // ... {BatteryCharacter} deals each non-hero target 2 lightning damage.
            return DealDamage(
                CharacterCard,
                c => c.Is().NonHero().Target().AccordingTo(this),
                2,
                DamageType.Lightning
            );
        }

        public bool HasBatteryUsedDischargeSinceStartOfLastTurn()
        {
            var lastStart = Journal.PhaseChangeEntries()
                .Where(pcje => pcje.ToPhase.IsStart && pcje.ToPhase.TurnTaker == TurnTaker)
                .LastOrDefault();

            var lastStartIndex = Journal.GetEntryIndex(lastStart) ?? 0;

            return Journal.UsePowerEntries()
                .Where(upje => GameController.IsDischargePower(upje, HeroTurnTaker) && Journal.GetEntryIndex(upje) >= lastStartIndex)
                .Any();
        }
    }
}
