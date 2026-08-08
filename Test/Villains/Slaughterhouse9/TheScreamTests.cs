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
    public class TheScreamTests : Slaughterhouse9TestBase
    {
        [Test()]
        public void TestDamageScalesWithEquipmentInPlay()
        {
            SetupNineGame();

            var crawler = PutMemberInPlay("CrawlerCharacter");
            RemoveCardTriggers(crawler);

            PlayCard("FlakCannon");
            PlayCard("GatlingGun");

            QuickHPStorage(legacy, bunker, haka);
            AssertDamageType(DamageType.Projectile);
            // Decline the offer to destroy equipment.
            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            PlayCard("TheScream", 0);

            // X = 2 x 2 equipment cards, dealt by the highest-HP Nine (Crawler).
            QuickHPCheck(-4, -4, -4);
        }

        [Test()]
        public void TestDestroyingEquipmentAvoidsDamage()
        {
            SetupNineGame();

            var crawler = PutMemberInPlay("CrawlerCharacter");
            RemoveCardTriggers(crawler);

            var flak = PlayCard("FlakCannon");

            QuickHPStorage(legacy, bunker, haka);
            // With no decisions set the framework picks the first option, so Bunker
            // destroys his only equipment (and the follow-up 0-damage target picks
            // stay unpolluted).

            PlayCard("TheScream", 0);

            AssertInTrash(flak);
            QuickHPCheck(0, 0, 0);
        }
    }
}
