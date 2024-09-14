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
    public class NotExactlySanitaryTests : ParahumanTest
    {
        [Test()]
        public void TestWorks()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "InsulaPrimalis");

            StartGame();

            PlayCard("NotExactlySanitary");

            QuickHPStorage(tempest);
            DealDamage(merchants, tempest, 1, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestDoesntTriggerToxic()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "InsulaPrimalis");

            StartGame();

            PlayCard("NotExactlySanitary");

            QuickHPStorage(tempest);
            DealDamage(merchants, tempest, 1, DamageType.Toxic);
            QuickHPCheck(-1);
        }
    }
}
