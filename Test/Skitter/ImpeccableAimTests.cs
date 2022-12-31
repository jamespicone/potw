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
    public class ImpeccableAimTests : BaseTest
    {
        protected HeroTurnTakerController skitter { get { return FindHero("Skitter"); } }
        protected HeroTurnTakerController tattletale { get { return FindHero("Tattletale"); } }

        [Test()]
        public void DealDamageToTwoTargets()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            var battalion = PlayCard("BladeBattalion");

            var gun = PlayCard("ImpeccableAim");

            DecisionSelectTargets = new Card[] { baron.CharacterCard, battalion };
            QuickHPStorage(baron.CharacterCard, battalion);

            UsePower(gun, 0);

            QuickHPCheck(-2, -1);
        }

        [Test()]
        public void DealDamageToOneTarget()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            var battalion = PlayCard("BladeBattalion");

            var gun = PlayCard("ImpeccableAim");

            DecisionSelectTargets = new Card[] { baron.CharacterCard, null };
            QuickHPStorage(baron.CharacterCard, battalion);

            UsePower(gun, 0);

            QuickHPCheck(-2, 0);
        }

        [Test()]
        public void DestroyEnvTarget()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            var police = PlayCard("PoliceBackup");

            var gun = PlayCard("ImpeccableAim");

            DecisionSelectCards = new Card[] { police };

            UsePower(gun, 1);

            AssertNotInPlay(police);
        }
    }
}
