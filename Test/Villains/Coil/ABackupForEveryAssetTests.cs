using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Coil
{
    [TestFixture()]
    public class ABackupForEveryAssetTests : CoilTestBase
    {
        [Test()]
        public void TestPutsTrashedOngoingsIntoPlay()
        {
            SetupCoilGame();
            RemoveCoilTriggers();
            CleanupSetupNoise();

            var twoBites = PutInTrash("TwoBitesAtEveryCherry");
            var plans = PutInTrash("PlansWithinPlans");
            var mercs = PutInTrash("Mercenaries");

            PlayCard("ABackupForEveryAsset");

            AssertIsInPlay(twoBites);
            AssertIsInPlay(plans);
            AssertInTrash(mercs);
        }
    }
}
