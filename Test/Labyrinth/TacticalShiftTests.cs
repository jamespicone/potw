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
    public class TacticalShiftTests : ParahumanTest
    {
        [Test()]
        public void TestDestroysOngoing()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Jp.ParahumansOfTheWormverse.Tattletale", "InsulaPrimalis");

            StartGame();

            var forcefield = PlayCard("LivingForceField");

            DecisionSelectCard = forcefield;

            AssertIsInPlay(forcefield);
            PlayCard("TacticalShift");
            AssertInTrash(forcefield);
        }

        [Test()]
        public void TestOptionalDestroyOngoing()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Jp.ParahumansOfTheWormverse.Tattletale", "InsulaPrimalis");

            StartGame();

            var forcefield = PlayCard("LivingForceField");

            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            AssertIsInPlay(forcefield);
            PlayCard("TacticalShift");
            AssertIsInPlay(forcefield);
        }

        [Test()]
        public void TestPlaysEnvironment()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Jp.ParahumansOfTheWormverse.Tattletale", "InsulaPrimalis");

            StartGame();

            var raptors = StackDeck("VelociraptorPack");

            PlayCard("TacticalShift");
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

            DecisionDoNotSelectCard = SelectionType.DestroyCard;
            PlayCard("TacticalShift");

            AssertIsInPlay(garden);
        }

        [Test()]
        public void TestDestroyingShapingStashesEnv()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Jp.ParahumansOfTheWormverse.Tattletale", "InsulaPrimalis");

            StartGame();

            PlayCard("ObsidianField");
            var garden = PlayCard("BeautifulGarden");
            var raptors = PlayCard("VelociraptorPack");

            AssertIsInPlay(garden);

            DecisionSelectCards = new Card[] { garden, raptors, null };
            PlayCard("TacticalShift");

            AssertUnderCard(labyrinth.CharacterCard, raptors);
            AssertInTrash(garden);
        }
    }
}
