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
    public class CoolToysTests : ParahumanTest
    {
        [Test()]
        public void TestUncharged()
        {
            SetupGameController("AkashBhuta", "Jp.ParahumansOfTheWormverse.Battery", "InsulaPrimalis");

            StartGame();

            PlayCard("GlowingThreads");

            DecisionSelectCard = akash.CharacterCard;

            QuickHPStorage(akash.CharacterCard);
            PlayCard("CoolToys");
            QuickHPCheck(0);
        }

        [Test()]
        public void TestChargedOneEquip()
        {
            SetupGameController("AkashBhuta", "Jp.ParahumansOfTheWormverse.Battery", "InsulaPrimalis");

            StartGame();

            UsePower(battery.CharacterCard, 0);

            PlayCard("GlowingThreads");

            DecisionSelectCard = akash.CharacterCard;

            QuickHPStorage(akash.CharacterCard);
            AssertDamageSource(battery.CharacterCard);
            AssertDamageType(DamageType.Lightning);
            PlayCard("CoolToys");
            QuickHPCheck(-1);

            Assert.That(battery.CharacterCardController.IsCharged(battery.CharacterCard), Is.False);
        }

        [Test()]
        public void TestChargedOneEquipOneDevice()
        {
            SetupGameController("AkashBhuta", "Jp.ParahumansOfTheWormverse.Battery", "Luminary", "InsulaPrimalis");

            StartGame();

            UsePower(battery.CharacterCard, 0);

            PlayCard("GlowingThreads");
            PlayCard("RepairNanites");

            DecisionSelectCard = akash.CharacterCard;

            QuickHPStorage(akash.CharacterCard);
            AssertDamageSource(battery.CharacterCard);
            AssertDamageType(DamageType.Lightning);
            PlayCard("CoolToys");
            QuickHPCheck(-2);

            Assert.That(battery.CharacterCardController.IsCharged(battery.CharacterCard), Is.False);
        }
    }
}
