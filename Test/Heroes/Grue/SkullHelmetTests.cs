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
    public class SkullHelmetTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "InsulaPrimalis");

            StartGame();
        }

        [Test()]
        public void TestTriggersWhenDarknessPlaced()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            PlayCard("SkullHelmet");

            DecisionYesNo = true;

            QuickHPStorage(baron);
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(baron.CharacterCard));
            QuickHPCheck(-1); // 1 psychic damage
        }

        [Test()]
        public void TestMayDeal1PsychicDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            PlayCard("SkullHelmet");

            DecisionYesNo = true;
            AssertDamageType(DamageType.Psychic);

            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(baron.CharacterCard));
        }

        [Test()]
        public void TestDamageIsOptional()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            PlayCard("SkullHelmet");

            DecisionYesNo = false;

            QuickHPStorage(baron);
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(baron.CharacterCard));
            QuickHPCheck(0); // No damage
        }

        [Test()]
        public void TestTriggersMultipleTimesPerTurn()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            PlayCard("SkullHelmet");

            DecisionYesNo = true;

            // First Darkness
            QuickHPStorage(baron);
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(baron.CharacterCard));
            QuickHPCheck(-1);

            // Second Darkness (on Bunker)
            QuickHPStorage(bunker);
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(bunker.CharacterCard));
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestIsEquipment()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            var helmet = GetCard("SkullHelmet");

            Assert.That(helmet.DoKeywordsContain("equipment"), Is.True, "Skull Helmet should be equipment");
        }

        [Test()]
        public void TestIsLimited()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            var helmet1 = PlayCard("SkullHelmet", 0);
            AssertIsInPlay(helmet1);

            var helmet2 = PlayCard("SkullHelmet", 1);
            // Second copy goes to trash, first stays in play
            AssertIsInPlay(helmet1);
            AssertInTrash(helmet2);
        }

        [Test()]
        public void TestTriggersOnAnyTarget()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            PlayCard("SkullHelmet");

            DecisionYesNo = true;

            // Darkness on hero
            QuickHPStorage(bunker);
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(bunker.CharacterCard));
            QuickHPCheck(-1);

            // Darkness on Grue himself
            QuickHPStorage(grue);
            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(grue.CharacterCard));
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestDamageSourceIsGrue()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            PlayCard("SkullHelmet");

            DecisionYesNo = true;
            AssertDamageSource(grue.CharacterCard);

            RunCoroutine(grue.CharacterCardController.PutDarknessIntoPlay(baron.CharacterCard));
        }
    }
}
