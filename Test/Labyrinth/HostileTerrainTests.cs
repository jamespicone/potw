using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Jp.ParahumansOfTheWormverse.Labyrinth;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.VoidGuardMainstay;
using Handelabra;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Labyrinth
{
    [TestFixture()]
    public class HostileTerrainTests : ParahumanTest
    {
        [Test()]
        public void TestDestroysTarget()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Jp.ParahumansOfTheWormverse.Tattletale", "InsulaPrimalis");

            StartGame();

            var battalion = PlayCard("BladeBattalion");

            DecisionSelectCard = battalion;

            // Stack the deck so an unlucky play doesn't stall out the test
            StackDeck("RiverOfLava");

            AssertIsInPlay(battalion);
            PlayCard("HostileTerrain");
            AssertInTrash(battalion);
        }

        [Test()]
        public void TestOptionalDestroyOngoing()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Jp.ParahumansOfTheWormverse.Tattletale", "InsulaPrimalis");

            StartGame();

            var battalion = PlayCard("BladeBattalion");

            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            // Stack the deck so an unlucky play doesn't stall out the test
            StackDeck("RiverOfLava");

            AssertIsInPlay(battalion);
            PlayCard("HostileTerrain");
            AssertIsInPlay(battalion);
        }

        [Test()]
        public void TestPlaysEnvironment()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Jp.ParahumansOfTheWormverse.Tattletale", "InsulaPrimalis");

            StartGame();

            var raptors = StackDeck("VelociraptorPack");

            PlayCard("HostileTerrain");
            AssertIsInPlay(raptors);
        }

        [Test()]
        public void TestDestroyingShapingOptional()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Jp.ParahumansOfTheWormverse.Tattletale", "InsulaPrimalis");

            StartGame();

            PlayCard("ObsidianField");
            var garden = PlayCard("BeautifulGarden");

            AssertIsInPlay(garden);

            // Stack the deck so an unlucky play doesn't stall out the test
            StackDeck("RiverOfLava");

            DecisionDoNotSelectCard = SelectionType.DestroyCard;
            PlayCard("HostileTerrain");

            AssertIsInPlay(garden);
        }

        [Test()]
        public void TestDestroyingShapingHurtsVillains()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Jp.ParahumansOfTheWormverse.Tattletale", "InsulaPrimalis");

            StartGame();
            RemoveMobileDefensePlatform();

            PlayCard("RiverOfLava");
            var garden = PlayCard("BeautifulGarden");

            var platform = PlayCard("MobileDefensePlatform");
            var battalion1 = PlayCard("BladeBattalion");
            var battalion2 = PlayCard("BladeBattalion");
            var battalion3 = PlayCard("BladeBattalion");

            var raptors = PlayCard("VelociraptorPack");

            AssertIsInPlay(garden);

            StackDeck("EnragedTRex");

            DecisionSelectCards = new Card[] { platform, garden, baron.CharacterCard, battalion1, battalion2, battalion3 };
            QuickHPStorage(battalion1, battalion2, battalion3, baron.CharacterCard, raptors);

            PlayCard("HostileTerrain");

            QuickHPCheck(-2, -2, -2, -2, 0);
        }
    }
}
