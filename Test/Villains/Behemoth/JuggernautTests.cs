using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Behemoth
{
    [TestFixture()]
    public class JuggernautTests : BehemothTestBase
    {
        [Test()]
        public void TestReducesDamageToBehemoth()
        {
            SetupBehemothGame();
            RemoveBehemothTriggers();
            ClearProximity();

            PlayCard("Juggernaut");

            QuickHPStorage(behemoth);
            DealDamage(haka, behemoth, 3, DamageType.Cold);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestHeroDamageMovesTokenToAttacker()
        {
            SetupBehemothGame();
            RemoveBehemothTriggers();
            ClearProximity();
            SetProximity(bunker, 1);

            PlayCard("Juggernaut");

            // Haka damages Behemoth; Bunker is the only other hero with a token.
            DealDamage(haka, behemoth, 3, DamageType.Melee);

            Assert.That(Proximity(haka).CurrentValue, Is.EqualTo(1));
            Assert.That(Proximity(bunker).CurrentValue, Is.EqualTo(0));
        }
    }
}
