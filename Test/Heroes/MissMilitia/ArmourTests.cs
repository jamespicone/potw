using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.MissMilitia
{
    [TestFixture()]
    public class ArmourTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "InsulaPrimalis");
        }

        [Test()]
        public void TestIsEquipmentLimited()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "InsulaPrimalis");
            StartGame();

            var card = GetCard("Armour");
            Assert.That(card.DoKeywordsContain("equipment"), Is.True);
            Assert.That(card.DoKeywordsContain("limited"), Is.True);
        }

        [Test()]
        public void TestPreventsDamage5OrMore()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            var armour = PlayCard("Armour");

            QuickHPStorage(missmilitia);
            DealDamage(baron, missmilitia, 5, DamageType.Melee);
            QuickHPCheck(0);

            AssertInTrash(armour);
        }

        [Test()]
        public void TestPreventsLargeDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            var armour = PlayCard("Armour");

            QuickHPStorage(missmilitia);
            DealDamage(baron, missmilitia, 10, DamageType.Melee);
            QuickHPCheck(0);

            AssertInTrash(armour);
        }

        [Test()]
        public void TestDoesNotPreventDamageBelow5()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            var armour = PlayCard("Armour");

            QuickHPStorage(missmilitia);
            DealDamage(baron, missmilitia, 4, DamageType.Melee);
            QuickHPCheck(-4);

            AssertIsInPlay(armour);
        }

        [Test()]
        public void TestDestroySelfOnPrevent()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            var armour = PlayCard("Armour");
            AssertIsInPlay(armour);

            DealDamage(baron, missmilitia, 6, DamageType.Melee);

            AssertInTrash(armour);
        }
    }
}
