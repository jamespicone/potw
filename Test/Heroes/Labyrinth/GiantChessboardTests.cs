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
    public class GiantChessboardTests : ParahumanTest
    {
        [Test()]
        public void TestEnvDestroyOptional()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Legacy", "InsulaPrimalis");

            StartGame();
            GoToUsePowerPhase(labyrinth);

            RemoveVillainCards();
            var battalion = PlayCard("BladeBattalion");

            var field = PlayCard("ObsidianField");
            var board = PlayCard("GiantChessboard");
            var raptors = PlayCard("VelociraptorPack");

            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            QuickHPStorage(baron.CharacterCard, labyrinth.CharacterCard, legacy.CharacterCard, raptors, battalion);
            GoToEndOfTurn(labyrinth);
            QuickHPCheck(0, 0, 0, 0, 0);

            AssertIsInPlay(raptors);
            AssertIsInPlay(field);
            AssertIsInPlay(board);
            AssertIsInPlay(battalion);
        }

        [Test()]
        public void TestDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Legacy", "InsulaPrimalis");

            StartGame();
            GoToUsePowerPhase(labyrinth);

            RemoveVillainCards();
            var battalion = PlayCard("BladeBattalion");

            var field = PlayCard("ObsidianField");
            var board = PlayCard("GiantChessboard");
            var raptors = PlayCard("VelociraptorPack");

            DecisionSelectCards = new Card[] { raptors, battalion, null };

            QuickHPStorage(baron.CharacterCard, labyrinth.CharacterCard, legacy.CharacterCard, battalion);
            GoToEndOfTurn(labyrinth);
            QuickHPCheck(0, 0, 0, -1);

            AssertInTrash(raptors);
            AssertIsInPlay(field);
            AssertIsInPlay(board);
            AssertIsInPlay(battalion);
        }

        [Test()]
        public void TestDamageShapingBased()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Legacy", "InsulaPrimalis");

            StartGame();
            GoToUsePowerPhase(labyrinth);

            RemoveVillainCards();
            var battalion = PlayCard("BladeBattalion");

            var field = PlayCard("ObsidianField");
            var board = PlayCard("GiantChessboard");

            var field2 = PlayCard("ObsidianField");
            DecisionSelectCard = field2;
            var garden = PlayCard("BeautifulGarden");
            ResetDecisions();

            var raptors = PlayCard("VelociraptorPack");

            DecisionSelectCards = new Card[] { raptors, battalion, null };

            QuickHPStorage(baron.CharacterCard, labyrinth.CharacterCard, legacy.CharacterCard, battalion);
            GoToEndOfTurn(labyrinth);
            QuickHPCheck(0, 0, 0, -2);

            AssertInTrash(raptors);
            AssertIsInPlay(field);
            AssertIsInPlay(field2);
            AssertIsInPlay(garden);
            AssertIsInPlay(board);
            AssertIsInPlay(battalion);
        }
    }
}
