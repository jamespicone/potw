using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Environment.CoilsBase
{
    [TestFixture()]
    public class ParahumanPrisonTests : CoilsBaseTestBase
    {
        [Test()]
        public void TestDestroyedTargetIsJailedAndFreedWhenPrisonFalls()
        {
            SetupCoilsBaseGame();

            var prison = PlayCard("ParahumanPrison");
            var mercs = PlayCard("Mercenaries");

            DealDamage(haka, mercs, 10, DamageType.Melee);

            Assert.That(mercs.Location, Is.EqualTo(prison.UnderLocation), "The destroyed target should be under the prison");

            DestroyCard(prison);

            AssertIsInPlay(mercs);
        }

        [Test()]
        public void TestOnlyFirstDestroyedTargetEachTurnIsJailed()
        {
            SetupCoilsBaseGame();

            var prison = PlayCard("ParahumanPrison");
            var mercs0 = PlayCard("Mercenaries", 0);
            var mercs1 = PlayCard("Mercenaries", 1);

            DealDamage(haka, mercs0, 10, DamageType.Melee);
            DealDamage(haka, mercs1, 10, DamageType.Melee);

            Assert.That(mercs0.Location, Is.EqualTo(prison.UnderLocation));
            AssertInTrash(mercs1);
        }

        [Test()]
        public void TestNewPrisonerFreesPrevious()
        {
            SetupCoilsBaseGame();

            var prison = PlayCard("ParahumanPrison");
            var mercs0 = PlayCard("Mercenaries", 0);
            var mercs1 = PlayCard("Mercenaries", 1);

            DealDamage(haka, mercs0, 10, DamageType.Melee);

            GoToStartOfTurn(legacy);

            DealDamage(haka, mercs1, 10, DamageType.Melee);

            Assert.That(mercs1.Location, Is.EqualTo(prison.UnderLocation));
            AssertIsInPlay(mercs0);
        }
    }
}
