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
    public class PyrokinesisTests : LungTestBase
    {
        [Test()]
        public void TestLungMeleeDamageAddsFireDamage()
        {
            SetupLungGame();
            RemoveLungTriggers();

            PlayCard("Pyrokinesis");

            QuickHPStorage(haka);
            // The bonus fire damage is dealt (and observed) inside the melee damage's
            // after-trigger, so the observer sees the fire action complete first.
            AssertDamageType(DamageType.Fire, DamageType.Melee);
            AssertDamageSource(lung.CharacterCard, lung.CharacterCard);

            DealDamage(lung.CharacterCard, haka.CharacterCard, 2, DamageType.Melee);

            QuickHPCheck(-4);
        }

        [Test()]
        public void TestNoBonusOnLungFireDamage()
        {
            SetupLungGame();
            RemoveLungTriggers();

            PlayCard("Pyrokinesis");

            QuickHPStorage(haka);
            DealDamage(lung.CharacterCard, haka.CharacterCard, 2, DamageType.Fire);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestNoBonusOnOtherVillainMeleeDamage()
        {
            SetupLungGame();
            RemoveLungTriggers();

            PlayCard("Pyrokinesis");
            var oniLee = PlayCard("OniLee");

            QuickHPStorage(haka);
            DealDamage(oniLee, haka.CharacterCard, 3, DamageType.Melee);
            QuickHPCheck(-3);
        }
    }
}
