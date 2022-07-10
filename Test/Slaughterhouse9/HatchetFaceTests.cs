using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Jp.ParahumansOfTheWormverse.Tattletale;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Slaughterhouse9
{
    [TestFixture()]
    public class HatchetFaceTests : BaseTest
    {
        protected Card hatchetFace { get { return FindCard(c => c.Identifier == "HatchetFace"); } }

        protected HeroTurnTakerController legend { get { return FindHero("Legend"); } }

        [Test()]
        public void TestPreventHeroPowerPhase()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Legend",
                "Megalopolis"
            );

            StartGame();
            GoToPlayCardPhase(legend);

            PlayCard("HatchetFace");

            EnterNextTurnPhase();

            AssertCannotPerformPhaseAction();
        }

        [Test()]
        public void TestPreventOtherPowerUses()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Legend",
                "Megalopolis"
            );

            StartGame();

            PlayCard("HatchetFace");

            AssertNoDecision();
            UsePower(legend);            
        }
    }
}
