using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Environment.NewDelhi
{
    [TestFixture()]
    public class HeroicSacrificeTests : NewDelhiTestBase
    {
        [Test()]
        public void TestRedirectedHeroDamageIncreased()
        {
            SetupNewDelhiGame();

            PlayCard("HeroicSacrifice");
            // Lead From The Front lets Legacy redirect villain damage from another
            // hero to himself — a hero-to-hero redirect.
            PlayCard("LeadFromTheFront");

            QuickHPStorage(legacy.CharacterCard, bunker.CharacterCard);
            DecisionYesNo = true;

            DealDamage(baron.CharacterCard, bunker.CharacterCard, 2, DamageType.Melee);

            // Redirected from Bunker to Legacy, increased by 2 — plus Baron Blade's
            // +1 nemesis bonus against Legacy.
            QuickHPCheck(-5, 0);
        }

        [Test()]
        public void TestDestroyedWhenHeroDamagesHero()
        {
            SetupNewDelhiGame();

            var sacrifice = PlayCard("HeroicSacrifice");

            DealDamage(haka, legacy, 1, DamageType.Melee);

            AssertInTrash(sacrifice);
        }
    }
}
