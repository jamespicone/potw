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
    public class BlindingTests : ParahumanTest
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
            PlayCard("Blinding");
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

            PlayCard("Blinding");
        }

        [Test()]
        public void TestWithDarkness_MayDestroyOngoing()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            // Put Darkness next to Baron
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(baron.CharacterCard));

            var ongoing = PlayCard("LivingForceField");
            AssertIsInPlay(ongoing);

            DecisionSelectTarget = baron.CharacterCard;
            DecisionSelectCard = ongoing;

            PlayCard("Blinding");

            AssertInTrash(ongoing);
        }

        [Test()]
        public void TestWithoutDarkness_NoDestruction()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            // No Darkness next to Baron
            var ongoing = PlayCard("LivingForceField");
            AssertIsInPlay(ongoing);

            DecisionSelectTarget = baron.CharacterCard;

            PlayCard("Blinding");

            // Ongoing should still be in play (no option to destroy)
            AssertIsInPlay(ongoing);
        }

        [Test()]
        public void TestOngoingDestructionOptional()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            // Put Darkness next to Baron
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(baron.CharacterCard));

            var ongoing = PlayCard("LivingForceField");
            AssertIsInPlay(ongoing);

            DecisionSelectTarget = baron.CharacterCard;
            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            PlayCard("Blinding");

            // Ongoing should still be in play (declined destruction)
            AssertIsInPlay(ongoing);
        }

        [Test()]
        public void TestDamageSourceIsGrue()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            DecisionSelectTarget = baron.CharacterCard;

            AssertDamageSource(grue.CharacterCard);
            PlayCard("Blinding");
        }

        [Test()]
        public void TestDamageTypeIsMelee()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            DecisionSelectTarget = baron.CharacterCard;

            AssertDamageType(DamageType.Melee);
            PlayCard("Blinding");
        }

        [Test()]
        public void TestIsOneShot()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            var blinding = GetCard("Blinding");

            Assert.That(blinding.DoKeywordsContain("one-shot"), Is.True, "Blinding should be a one-shot");
        }
    }
}
