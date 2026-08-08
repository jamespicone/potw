using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Slaughterhouse9
{
    [TestFixture()]
    public class SomeLightEntertainmentTests : Slaughterhouse9TestBase
    {
        [Test()]
        public void TestEachVillainTargetHitsHighestHero()
        {
            SetupNineGame();

            var jack = PutMemberInPlay("JackSlashCharacter");
            var bonesaw = PutMemberInPlay("BonesawCharacter");

            // Their once-per-turn trash reactions would fire when this attack card
            // hits the trash.
            RemoveCardTriggers(jack, bonesaw);

            SetHitPoints(legacy, 20);
            SetHitPoints(bunker, 15);
            SetHitPoints(haka, 25);

            QuickHPStorage(legacy, bunker, haka);

            PlayCard("SomeLightEntertainment", 0);

            // 2 melee from each of the two villain targets.
            QuickHPCheck(0, 0, -4);
        }
    }
}
