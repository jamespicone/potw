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
    public class LeathersTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "InsulaPrimalis");

            StartGame();
        }

        [Test()]
        public void TestReducesNonPsychicDamageToGrue()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            PlayCard("Leathers");

            // Melee damage is reduced
            QuickHPStorage(grue);
            DealDamage(baron, grue, 3, DamageType.Melee);
            QuickHPCheck(-2); // 3 - 1 = 2

            // Fire damage is reduced
            QuickHPStorage(grue);
            DealDamage(baron, grue, 3, DamageType.Fire);
            QuickHPCheck(-2);

            // Energy damage is reduced
            QuickHPStorage(grue);
            DealDamage(baron, grue, 3, DamageType.Energy);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestDoesNotReducePsychicDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            PlayCard("Leathers");

            // Psychic damage is NOT reduced
            QuickHPStorage(grue);
            DealDamage(baron, grue, 3, DamageType.Psychic);
            QuickHPCheck(-3); // Full damage
        }

        [Test()]
        public void TestIsLimited()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            var leathers1 = PlayCard("Leathers", 0);
            AssertIsInPlay(leathers1);

            var leathers2 = PlayCard("Leathers", 1);
            // Second copy goes to trash, first stays in play
            AssertIsInPlay(leathers1);
            AssertInTrash(leathers2);
        }

        [Test()]
        public void TestStacksWithDarkness()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            PlayCard("Leathers");

            // Put Darkness next to Grue
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(grue.CharacterCard));

            // First damage should be reduced by both Leathers (-1) and Darkness (-1)
            QuickHPStorage(grue);
            DealDamage(baron, grue, 4, DamageType.Melee);
            QuickHPCheck(-2); // 4 - 1 - 1 = 2
        }

        [Test()]
        public void TestOnlyAffectsGrue()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            PlayCard("Leathers");

            // Damage to Bunker is NOT reduced
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 3, DamageType.Melee);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestIsEquipment()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            var leathers = GetCard("Leathers");

            Assert.That(leathers.DoKeywordsContain("equipment"), Is.True, "Leathers should be equipment");
        }
    }
}
