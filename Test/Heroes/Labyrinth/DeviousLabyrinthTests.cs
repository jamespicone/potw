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
    public class DeviousLabyrinthTests : ParahumanTest
    {
        [Test()]
        public void TestBlanksTargets()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Legacy", "InsulaPrimalis");

            StartGame();
            GoToUsePowerPhase(labyrinth);

            RemoveVillainCards();
            RemoveVillainTriggers();

            var field = PlayCard("ObsidianField");
            var raptors = PlayCard("VelociraptorPack");
            DecisionSelectCard = field;
            var board = PlayCard("DeviousLabyrinth");
            ResetDecisions();

            DecisionSelectCards = new Card[] { raptors, null };
            var battalion = PlayCard("BladeBattalion");
            ResetDecisions();

            Assert.That(battalion.IsBlank, Is.True);
            Assert.That(GameController.IsInhibited(FindCardController(battalion)), Is.True);

            GoToStartOfTurn(labyrinth);

            Assert.That(battalion.IsBlank, Is.False);
            Assert.That(GameController.IsInhibited(FindCardController(battalion)), Is.False);
        }
    }
}
