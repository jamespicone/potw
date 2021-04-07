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
    public class MissMilitiaUtilityCardController : CardController
    {
        public const string MacheteKey = "Machete";
        public const string PistolKey = "Pistol";
        public const string SubmachineGunKey = "Submachine Gun";
        public const string SniperRifleKey = "Sniper Rifle";
        public string[] AllWeapons = { MacheteKey, PistolKey, SubmachineGunKey, SniperRifleKey };

        public MissMilitiaUtilityCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public bool HasMissMilitiaStartedMoreThanOneTurnThisGame()
        {
            return (from pcje in Journal.PhaseChangeEntries() where pcje.ToPhase != null && pcje.ToPhase.TurnTaker == base.TurnTaker && pcje.ToPhase.IsStart select pcje).Count() > 1;
        }

        public bool IsMissMilitiaUsingWeaponPower(UsePowerJournalEntry upje, string weaponKey)
        {
            return upje.PowerUser == base.HeroTurnTaker && upje.CardWithPower.Title == weaponKey && upje.CardWithPower.Owner == base.TurnTaker;
        }

        public bool HasUsedWeaponSinceStartOfLastTurn(string weaponKey)
        {
            if (AllWeapons.Contains(weaponKey))
            {
                if (HasMissMilitiaStartedMoreThanOneTurnThisGame())
                {
                    // Find the last time Miss Militia started her turn that wasn't the start of the current turn...
                    PhaseChangeJournalEntry startOfThisTurn = Journal.PhaseChangeEntries().Where((PhaseChangeJournalEntry pcje) => pcje.ToPhase.IsStart).LastOrDefault();
                    PhaseChangeJournalEntry startOfMissMilitiasLastTurn = Journal.PhaseChangeEntries().Where((PhaseChangeJournalEntry pcje) => pcje.ToPhase.IsStart && pcje.ToPhase.TurnTaker == base.TurnTaker && pcje != startOfThisTurn).LastOrDefault();
                    // Find all UsePowerJournalEntries that match IsMissMilitiaUsingWeaponPower(weaponKey) and take place between startOfMissMilitiasLastTurn and now
                    IEnumerable<UsePowerJournalEntry> matchingPowersSinceStartOfLastTurn = Journal.QueryJournalEntries<UsePowerJournalEntry>((UsePowerJournalEntry upje) => Journal.GetEntryIndex(upje) > Journal.GetEntryIndex(startOfMissMilitiasLastTurn) && IsMissMilitiaUsingWeaponPower(upje, weaponKey));
                    // If there's at least 1, return true
                    if (matchingPowersSinceStartOfLastTurn != null && matchingPowersSinceStartOfLastTurn.Count() > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public string ShowWeaponStatus(string weaponKey)
        {
            if (AllWeapons.Contains(weaponKey))
            {
                if (!HasMissMilitiaStartedMoreThanOneTurnThisGame())
                {
                    return base.TurnTaker.Name + " has not used the power on " + weaponKey + " since " + base.TurnTaker.Name + " has not had a previous start of turn this game.";
                }
                bool active = HasUsedWeaponSinceStartOfLastTurn(weaponKey);
                if (active)
                {
                    return base.TurnTaker.Name + " has used the power on " + weaponKey + " since the start of her last turn.";
                }
                else
                {
                    return base.TurnTaker.Name + " has not used the power on " + weaponKey + " since the start of her last turn.";
                }
            }
            return "";
        }

        public void ShowWeaponStatusIfActive(string iconKey)
        {
            SpecialStringMaker.ShowSpecialString(() => ShowWeaponStatus(iconKey)).Condition = () => HasUsedWeaponSinceStartOfLastTurn(iconKey);
        }

        public string UsedWeaponList()
        {
            if (!HasMissMilitiaStartedMoreThanOneTurnThisGame())
            {
                return base.TurnTaker.Name + " has not had a previous start of turn this game.";
            }

            List<string> usedWeapons = new List<string>();
            foreach(string key in AllWeapons)
            {
                if (HasUsedWeaponSinceStartOfLastTurn(key))
                {
                    usedWeapons.Add(key);
                }
            }

            if (usedWeapons.Count() > 0)
            {
                return base.TurnTaker.Name + "'s " + usedWeapons.Count().ToString_SingularOrPlural("Weapon", "Weapons") + " used since the start of her last turn: " + usedWeapons.ToCommaList() + ".";
            }
            else
            {
                return base.TurnTaker.Name + " has not used any Weapon powers since the start of her last turn.";
            }
        }
    }
}
