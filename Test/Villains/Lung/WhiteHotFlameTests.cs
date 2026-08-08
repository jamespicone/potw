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
    public class WhiteHotFlameTests : LungTestBase
    {
        [Test()]
        public void TestLungFireDamageIsIrreducible()
        {
            SetupLungGame();
            RemoveLungTriggers();

            PlayCard("WhiteHotFlame");
            PlayCard("Fortitude"); // Reduce damage dealt to Legacy by 1

            QuickHPStorage(legacy);
            AssertIrreducible();

            DealDamage(lung.CharacterCard, legacy.CharacterCard, 2, DamageType.Fire);

            // Fortitude couldn't reduce it.
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestLungMeleeDamageIsStillReducible()
        {
            SetupLungGame();
            RemoveLungTriggers();

            PlayCard("WhiteHotFlame");
            PlayCard("Fortitude");

            QuickHPStorage(legacy);
            AssertNotIrreducible();

            DealDamage(lung.CharacterCard, legacy.CharacterCard, 2, DamageType.Melee);

            QuickHPCheck(-1);
        }

        [Test()]
        public void TestOtherSourceFireDamageIsStillReducible()
        {
            SetupLungGame();
            RemoveLungTriggers();

            PlayCard("WhiteHotFlame");
            PlayCard("Fortitude");

            QuickHPStorage(legacy);
            AssertNotIrreducible();

            DealDamage(bunker.CharacterCard, legacy.CharacterCard, 2, DamageType.Fire);

            QuickHPCheck(-1);
        }
    }
}
