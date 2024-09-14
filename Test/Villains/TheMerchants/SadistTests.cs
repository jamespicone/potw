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
    public class SadistTests : ParahumanTest
    {
        [Test()]
        public void HitsLowest()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "Legacy", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            var sadist = PlayCard("Sadist");

            SetHitPoints(tempest, 15);
            SetHitPoints(legacy, 16);
            QuickHPStorage(tempest, legacy);
            AssertDamageSource(sadist);
            AssertDamageType(DamageType.Melee);
            GoToEndOfTurn();
            QuickHPCheck(-2, 0);
        }
    }
}
