using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Jp.ParahumansOfTheWormverse.Labyrinth;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Labyrinth
{
    [TestFixture()]
    public class TheAsylumTests : ParahumanTest
    {
        [Test()]
        public void TestDoesDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "InsulaPrimalis");

            StartGame();
            GoToUsePowerPhase(labyrinth);

            RemoveVillainCards();
            var battalion = PlayCard("BladeBattalion");

            PlayCard("ObsidianField");
            PlayCard("TheAsylum");

            QuickHPStorage(labyrinth.CharacterCard, baron.CharacterCard, battalion);
            GoToEndOfTurn(labyrinth);
            QuickHPCheck(-3, -2, -2);
        }

        [Test()]
        public void TestSelfDamageIsIrreducible()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "InsulaPrimalis");

            StartGame();
            GoToUsePowerPhase(labyrinth);

            RemoveVillainCards();
            var battalion = PlayCard("BladeBattalion");

            PlayCard("ObsidianField");
            PlayCard("TheAsylum");

            // Defensive Buttress reduces damage dealt to hero targets by 1, but
            // The Asylum's self-damage is irreducible so Labyrinth still takes 3.
            StackDeck("RiverOfLava"); // harmless card for Buttress's environment play
            DecisionDoNotSelectCard = SelectionType.DestroyCard;
            PlayCard("DefensiveButtress");
            ResetDecisions();

            QuickHPStorage(labyrinth.CharacterCard, baron.CharacterCard, battalion);
            GoToEndOfTurn(labyrinth);
            QuickHPCheck(-3, -2, -2);
        }
    }
}
