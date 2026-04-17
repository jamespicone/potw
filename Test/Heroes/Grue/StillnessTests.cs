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
    public class StillnessTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "InsulaPrimalis");

            StartGame();
        }

        [Test()]
        public void TestReducesFireDamageBy1()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            PlayCard("Stillness");

            // Put Darkness next to Bunker
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(bunker.CharacterCard));

            // Fire damage to Bunker should be reduced
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 3, DamageType.Fire);
            QuickHPCheck(-1); // 3 - 1 (Stillness) - 1 (Darkness TO) = 1
        }

        [Test()]
        public void TestReducesSonicDamageBy1()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            PlayCard("Stillness");

            // Put Darkness next to Bunker
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(bunker.CharacterCard));

            // Sonic damage to Bunker should be reduced
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 3, DamageType.Sonic);
            QuickHPCheck(-1); // 3 - 1 (Stillness) - 1 (Darkness TO) = 1
        }

        [Test()]
        public void TestReducesLightningDamageBy1()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            PlayCard("Stillness");

            // Put Darkness next to Bunker
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(bunker.CharacterCard));

            // Lightning damage to Bunker should be reduced
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 3, DamageType.Lightning);
            QuickHPCheck(-1); // 3 - 1 (Stillness) - 1 (Darkness TO) = 1
        }

        [Test()]
        public void TestReducesEnergyDamageBy1()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            PlayCard("Stillness");

            // Put Darkness next to Bunker
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(bunker.CharacterCard));

            // Energy damage to Bunker should be reduced
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 3, DamageType.Energy);
            QuickHPCheck(-1); // 3 - 1 (Stillness) - 1 (Darkness TO) = 1
        }

        [Test()]
        public void TestReducesDamageDealtByTargetWithDarkness()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            PlayCard("Stillness");

            // Put Darkness next to Baron (source)
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(baron.CharacterCard));

            // Fire damage BY Baron should be reduced by Stillness
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 3, DamageType.Fire);
            QuickHPCheck(-1); // 3 - 1 (Stillness) - 1 (Darkness FROM) = 1
        }

        [Test()]
        public void TestRequiresDarknessAdjacent()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            PlayCard("Stillness");

            // NO Darkness next to Bunker
            // Stillness should NOT reduce damage
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 3, DamageType.Fire);
            QuickHPCheck(-3); // Full damage
        }

        [Test()]
        public void TestStartOfTurn_PlacesDarkness()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveVillainTriggers();

            PlayCard("Stillness");

            DecisionSelectCard = bunker.CharacterCard;

            GoToStartOfTurn(grue);

            // Should have placed Darkness next to Bunker
            var darknessNextToBunker = bunker.CharacterCard.GetAllNextToCards(false).Where(c => c.Identifier == "Darkness");
            Assert.That(darknessNextToBunker.Count(), Is.EqualTo(1), "Should have 1 Darkness next to Bunker");
        }

        [Test()]
        public void TestIsLimited()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            var stillness1 = PlayCard("Stillness", 0);
            AssertIsInPlay(stillness1);

            var stillness2 = PlayCard("Stillness", 1);
            // Second copy goes to trash, first stays in play
            AssertIsInPlay(stillness1);
            AssertInTrash(stillness2);
        }

        [Test()]
        public void TestDoesNotAffectMeleeDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            PlayCard("Stillness");

            // Put Darkness next to Bunker
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(bunker.CharacterCard));

            // Melee damage is NOT reduced by Stillness (only by Darkness TO)
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 3, DamageType.Melee);
            QuickHPCheck(-2); // 3 - 1 (Darkness TO) = 2, Stillness doesn't apply
        }

        [Test()]
        public void TestIsOngoing()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            var stillness = GetCard("Stillness");

            Assert.That(stillness.DoKeywordsContain("ongoing"), Is.True, "Stillness should be ongoing");
        }

        [Test()]
        public void TestReductionOnlyAppliesOnce()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            PlayCard("Stillness");

            // Put Darkness next to both Baron and Bunker
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(baron.CharacterCard));
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(bunker.CharacterCard));

            // Fire damage from Baron to Bunker - both have Darkness
            // Stillness should only reduce once (not twice)
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 4, DamageType.Fire);
            // 4 - 1 (Stillness) - 1 (Darkness FROM) - 1 (Darkness TO) = 1
            QuickHPCheck(-1);
        }
    }
}
