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
    public class SmashTests : LungTestBase
    {
        [Test()]
        public void TestDamagesLowestHeroAndDestroysCard()
        {
            SetupLungGame();
            RemoveLungTriggers();

            var presence = PlayCard("InspiringPresence");

            SetHitPoints(legacy, 20);
            SetHitPoints(bunker, 15);
            SetHitPoints(haka, 25);

            QuickHPStorage(legacy, bunker, haka);
            AssertDamageSource(lung.CharacterCard);
            AssertDamageType(DamageType.Melee);
            DecisionSelectCard = presence;

            PlayCard("Smash");

            QuickHPCheck(0, -2, 0);
            AssertInTrash(presence);
        }
    }
}
