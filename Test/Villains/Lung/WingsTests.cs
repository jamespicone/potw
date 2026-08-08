using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Lung
{
    [TestFixture()]
    public class WingsTests : LungTestBase
    {
        [Test()]
        public void TestLungImmuneToEnvironmentDamage()
        {
            SetupLungGame();
            RemoveLungTriggers();

            PlayCard("Wings");
            var pileup = PlayCard("TrafficPileup");

            QuickHPStorage(lung);
            DealDamage(pileup, lung.CharacterCard, 4, DamageType.Melee);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestLungNotImmuneToHeroDamage()
        {
            SetupLungGame();
            RemoveLungTriggers();

            PlayCard("Wings");

            QuickHPStorage(lung);
            DealDamage(haka, lung, 4, DamageType.Melee);
            QuickHPCheck(-4);
        }

        [Test()]
        public void TestHeroesNotProtectedFromEnvironmentDamage()
        {
            SetupLungGame();
            RemoveLungTriggers();

            PlayCard("Wings");
            var pileup = PlayCard("TrafficPileup");

            QuickHPStorage(haka);
            DealDamage(pileup, haka.CharacterCard, 4, DamageType.Melee);
            QuickHPCheck(-4);
        }
    }
}
