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
    public class StrangerAndMasterProtocolsTests : CoilsBaseTestBase
    {
        [Test()]
        public void TestHeroCardsCannotAffectOtherHeroes()
        {
            SetupCoilsBaseGame();

            PlayCard("StrangerAndMasterProtocols");

            // Inspiring Presence (Legacy) increases damage dealt by hero targets.
            PlayCard("InspiringPresence");
            var mercs = PlayCard("Mercenaries");

            QuickHPStorage(mercs);

            // Legacy is boosted by his own card...
            DealDamage(legacy.CharacterCard, mercs, 2, DamageType.Melee);
            QuickHPCheck(-3);

            // ... but Haka is not.
            DealDamage(haka.CharacterCard, mercs, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }
    }
}
