using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Jp.ParahumansOfTheWormverse.Skitter;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Skitter
{
    [TestFixture()]
    public class SweepTheAreaTests : BaseTest
    {
        protected HeroTurnTakerController skitter { get { return FindHero("Skitter"); } }
        protected HeroTurnTakerController tattletale { get { return FindHero("Tattletale"); } }

        [Test()]
        public void TestPower()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            var stacked = StackDeck("LivingForceField", "BladeBattalion", "MobileDefensePlatform");
            var sweep = PlayCard("SweepTheArea");

            QuickHPStorage(baron);

            UsePower(sweep, 0);

            QuickHPCheck(-1);

            AssertOnTopOfDeck(stacked.First());
            AssertInTrash(stacked.Skip(1));
        }
    }
}
