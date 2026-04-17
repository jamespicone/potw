using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Grue
{
    [TestFixture()]
    public class MartialTalentTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "InsulaPrimalis");

            StartGame();
        }

        [Test()]
        public void TestIncreasesMeleeDamageBy1()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            PlayCard("MartialTalent");

            // Grue's melee damage is increased
            QuickHPStorage(baron);
            DealDamage(grue, baron, 2, DamageType.Melee);
            QuickHPCheck(-3); // 2 + 1 = 3
        }

        [Test()]
        public void TestDoesNotIncreaseNonMelee()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            PlayCard("MartialTalent");

            // Fire damage is NOT increased
            QuickHPStorage(baron);
            DealDamage(grue, baron, 2, DamageType.Fire);
            QuickHPCheck(-2); // Just 2

            // Projectile damage is NOT increased
            QuickHPStorage(baron);
            DealDamage(grue, baron, 2, DamageType.Projectile);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestPower_Deals2MeleeDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            var martialTalent = PlayCard("MartialTalent");

            DecisionSelectTarget = baron.CharacterCard;

            QuickHPStorage(baron);
            UsePower(martialTalent);
            QuickHPCheck(-3); // 2 + 1 from MartialTalent passive = 3
        }

        [Test()]
        public void TestPower_DamageSourceIsGrue()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            var martialTalent = PlayCard("MartialTalent");

            DecisionSelectTarget = baron.CharacterCard;

            AssertDamageSource(grue.CharacterCard);
            UsePower(martialTalent);
        }

        [Test()]
        public void TestPower_DamageTypeIsMelee()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            var martialTalent = PlayCard("MartialTalent");

            DecisionSelectTarget = baron.CharacterCard;

            AssertDamageType(DamageType.Melee);
            UsePower(martialTalent);
        }

        [Test()]
        public void TestIsLimited()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            var talent1 = PlayCard("MartialTalent", 0);
            AssertIsInPlay(talent1);

            var talent2 = PlayCard("MartialTalent", 1);
            // Second copy goes to trash, first stays in play
            AssertIsInPlay(talent1);
            AssertInTrash(talent2);
        }

        [Test()]
        public void TestOnlyAffectsGrue()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            PlayCard("MartialTalent");

            // Bunker's melee damage is NOT increased
            QuickHPStorage(baron);
            DealDamage(bunker, baron, 2, DamageType.Melee);
            QuickHPCheck(-2); // Just 2
        }

        [Test()]
        public void TestIsOngoing()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            var martialTalent = GetCard("MartialTalent");

            Assert.That(martialTalent.DoKeywordsContain("ongoing"), Is.True, "Martial Talent should be ongoing");
        }
    }
}
