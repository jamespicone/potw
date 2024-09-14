using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.TheMerchants
{
    [TestFixture()]
    public class RevellerTests : ParahumanTest
    {
        [Test()]
        public void HitsHighest()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "Legacy", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            var reveller = PlayCard("Reveller");

            SetHitPoints(tempest, 15);
            SetHitPoints(legacy, 16);
            QuickHPStorage(tempest, legacy);
            AssertDamageSource(reveller);
            AssertDamageType(DamageType.Melee);
            GoToEndOfTurn();
            QuickHPCheck(0, -1);
        }
    }
}
