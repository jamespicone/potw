using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;
using Jp.ParahumansOfTheWormverse.Battery;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Battery
{
    [TestFixture()]
    public class GlowingThreadsTests : ParahumanTest
    {
        [Test()]
        public void TestBatteryPower()
        {
            SetupGameController("AkashBhuta", "Jp.ParahumansOfTheWormverse.Battery", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            PlayCard("GlowingThreads");

            QuickHPStorage(battery.CharacterCard);

            UsePower(battery, 0);
            Assert.IsTrue(battery.CharacterCardController.IsCharged(battery.CharacterCard));

            // Charge effect
            AssertNumberOfStatusEffectsInPlay(1);

            DealDamage(battery, battery, 2, DamageType.Infernal);
            QuickHPCheck(-2);

            UsePower(battery, 0);
            Assert.IsFalse(battery.CharacterCardController.IsCharged(battery.CharacterCard));

            // - charge effect, + damage reduce effect
            AssertNumberOfStatusEffectsInPlay(1);

            DealDamage(battery, battery, 2, DamageType.Infernal);
            QuickHPCheck(0);

            DealDamage(battery, battery, 2, DamageType.Infernal);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestCauldronBatteryPower()
        {
            SetupGameController("AkashBhuta", "Jp.ParahumansOfTheWormverse.Battery/BatteryCauldronCapeCharacter", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            PlayCard("GlowingThreads");

            StackDeck("NoTimeToRest");

            QuickHPStorage(battery.CharacterCard);

            UsePower(battery, 0);
            Assert.IsFalse(battery.CharacterCardController.IsCharged(battery.CharacterCard));

            DealDamage(battery, battery, 2, DamageType.Infernal);
            QuickHPCheck(-2);

            UsePower(battery, 1);
            Assert.IsTrue(battery.CharacterCardController.IsCharged(battery.CharacterCard));

            // + charge effect, + damage reduce effect, + delayed discharge effect
            AssertNumberOfStatusEffectsInPlay(3);

            DealDamage(battery, battery, 2, DamageType.Infernal);
            QuickHPCheck(0);

            DealDamage(battery, battery, 2, DamageType.Infernal);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestNotPowerDischargeDoesntTrigger()
        {
            SetupGameController("AkashBhuta", "Jp.ParahumansOfTheWormverse.Battery", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            PlayCard("GlowingThreads");

            QuickHPStorage(battery.CharacterCard);

            UsePower(battery, 0);
            Assert.IsTrue(battery.CharacterCardController.IsCharged(battery.CharacterCard));

            PlayCard("CoolToys");

            Assert.IsFalse(battery.CharacterCardController.IsCharged(battery.CharacterCard));

            DealDamage(battery, battery, 2, DamageType.Infernal);
            QuickHPCheck(-2);

            AssertNumberOfStatusEffectsInPlay(0);
        }
    }
}
