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
    public class DeviousLabyrinthTests : BaseTest
    {
        protected HeroTurnTakerController labyrinth { get { return FindHero("Labyrinth"); } }
        protected HeroTurnTakerController tattletale { get { return FindHero("Tattletale"); } }

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

            Assert.IsTrue(battalion.IsBlank);
            Assert.IsTrue(GameController.IsInhibited(FindCardController(battalion)));

            GoToStartOfTurn(labyrinth);

            Assert.IsFalse(battalion.IsBlank);
            Assert.IsFalse(GameController.IsInhibited(FindCardController(battalion)));
        }
    }
}
