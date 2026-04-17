using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;
using Jp.ParahumansOfTheWormverse.Grue;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Grue
{
    [TestFixture()]
    public class HandToHandAssaultTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "InsulaPrimalis");

            StartGame();
        }

        [Test()]
        public void TestDeals2MeleeDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            DecisionSelectTarget = baron.CharacterCard;

            QuickHPStorage(baron);
            PlayCard("HandToHandAssault");
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestOnlyTargetsNonHero()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            // Bunker should not be a valid target
            AssertNextDecisionChoices(
                included: new Card[] { baron.CharacterCard },
                notIncluded: new Card[] { bunker.CharacterCard, grue.CharacterCard }
            );

            PlayCard("HandToHandAssault");
        }

        [Test()]
        public void TestWithDarkness_DealsAnother2()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            // Put Darkness next to Baron
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(baron.CharacterCard));

            DecisionSelectTarget = baron.CharacterCard;

            QuickHPStorage(baron);
            PlayCard("HandToHandAssault");
            QuickHPCheck(-4); // 2 + 2 bonus = 4 (minus 1 from Darkness TO for each hit = 2)
        }

        [Test()]
        public void TestWithoutDarkness_Only2Damage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            // No Darkness next to Baron

            DecisionSelectTarget = baron.CharacterCard;

            QuickHPStorage(baron);
            PlayCard("HandToHandAssault");
            QuickHPCheck(-2); // Only 2 damage
        }

        [Test()]
        public void TestTotalDamageIs4WithDarkness()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            // Put Darkness next to Baron
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(baron.CharacterCard));

            DecisionSelectTarget = baron.CharacterCard;

            QuickHPStorage(baron);
            PlayCard("HandToHandAssault");
            // First hit: 2 damage
            // Second hit: 2 damage (bonus from Darkness being adjacent)
            // Note: Grue's damage is not reduced by Darkness
            // Total: 4
            QuickHPCheck(-4);
        }

        [Test()]
        public void TestDamageSourceIsGrue()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            DecisionSelectTarget = baron.CharacterCard;

            AssertDamageSource(grue.CharacterCard, grue.CharacterCard);
            PlayCard("HandToHandAssault");
        }

        [Test()]
        public void TestDamageTypeIsMelee()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            DecisionSelectTarget = baron.CharacterCard;

            AssertDamageType(DamageType.Melee);
            PlayCard("HandToHandAssault");
        }

        [Test()]
        public void TestIsOneShot()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            var card = GetCard("HandToHandAssault");

            Assert.That(card.DoKeywordsContain("one-shot"), Is.True, "Hand-to-Hand Assault should be a one-shot");
        }
    }
}
