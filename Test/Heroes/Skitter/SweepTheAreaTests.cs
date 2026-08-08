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
    public class SweepTheAreaTests : ParahumanTest
    {
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

        [Test()]
        public void TestPowerReturnsNonTargetsAndHitsAllVillainTargets()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            var battalion = PlayCard("BladeBattalion");
            var sweep = PlayCard("SweepTheArea");

            StackDeck(skitter, new[] { "AlwaysPlanning", "Regroup", "ImpeccableAim" });
            var topCards = skitter.TurnTaker.Deck.GetTopCards(3).ToList();

            DecisionSelectLocation = new LocationChoice(skitter.TurnTaker.Deck);

            QuickHPStorage(baron.CharacterCard, battalion);

            UsePower(sweep, 0);

            QuickHPCheck(-1, -1);

            // No targets among the revealed cards, so all three return to the
            // deck. The engine auto-decides the return order, so only assert
            // the contents.
            Assert.That(skitter.TurnTaker.Deck.GetTopCards(3), Is.EquivalentTo(topCards));
        }
    }
}
