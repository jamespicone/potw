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
    public class DarkenTheAreaTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "InsulaPrimalis");

            StartGame();
        }

        [Test()]
        public void TestPlacesDarknessOnEnvTargetEntry()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            PlayCard("DarkenTheArea");

            // Play an environment target
            var envTarget = PlayCard("VelociraptorPack");

            // Should have Darkness next to it
            var darknessNextToEnv = envTarget.GetAllNextToCards(false).Where(c => c.Identifier == "Darkness");
            Assert.That(darknessNextToEnv.Count(), Is.EqualTo(1), "Env target should have Darkness");
        }

        [Test()]
        public void TestEffectLastsUntilStartOfNextTurn()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveVillainTriggers();

            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            PlayCard("DarkenTheArea");

            GoToStartOfTurn(grue);

            // Effect should have expired - playing env target now should NOT get Darkness
            var envTarget = PlayCard("VelociraptorPack");

            var darknessNextToEnv = envTarget.GetAllNextToCards(false).Where(c => c.Identifier == "Darkness");
            Assert.That(darknessNextToEnv.Count(), Is.EqualTo(0), "Env target should NOT have Darkness after effect expires");
        }

        [Test()]
        public void TestTriggersMultipleTimes()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            PlayCard("DarkenTheArea");

            // Play two environment TARGETS (not just cards - must have HP)
            var envTarget1 = PlayCard("VelociraptorPack");
            var envTarget2 = PlayCard("EnragedTRex");

            // Both should have Darkness
            var darknessNextToEnv1 = envTarget1.GetAllNextToCards(false).Where(c => c.Identifier == "Darkness");
            Assert.That(darknessNextToEnv1.Count(), Is.EqualTo(1), "First env target should have Darkness");

            var darknessNextToEnv2 = envTarget2.GetAllNextToCards(false).Where(c => c.Identifier == "Darkness");
            Assert.That(darknessNextToEnv2.Count(), Is.EqualTo(1), "Second env target should have Darkness");
        }

        [Test()]
        public void TestMayDestroyEnvironmentCard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            // Go to Grue's play phase to avoid any villain/env turn effects
            GoToPlayCardPhase(grue);

            var envCard = PlayCard("VelociraptorPack");
            AssertIsInPlay(envCard);

            // DarkenTheArea only has one decision: optional env destruction
            DecisionSelectCard = envCard;

            PlayCard("DarkenTheArea");

            AssertInTrash(envCard);
        }

        [Test()]
        public void TestEnvironmentDestructionOptional()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            var envCard = PlayCard("VelociraptorPack");
            AssertIsInPlay(envCard);

            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            PlayCard("DarkenTheArea");

            // Environment card should still be in play
            AssertIsInPlay(envCard);
        }

        [Test()]
        public void TestOnlyAffectsEnvironmentTargets()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            PlayCard("DarkenTheArea");

            // Play a villain target
            var battalionCard = PutIntoPlay("BladeBattalion");

            // Should NOT have Darkness (it's villain, not environment)
            var darknessNextToVillain = battalionCard.GetAllNextToCards(false).Where(c => c.Identifier == "Darkness");
            Assert.That(darknessNextToVillain.Count(), Is.EqualTo(0), "Villain target should NOT have Darkness");
        }

        [Test()]
        public void TestIsOneShot()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Grue", "Bunker", "InsulaPrimalis");

            StartGame();

            var card = GetCard("DarkenTheArea");

            Assert.That(card.DoKeywordsContain("one-shot"), Is.True, "Darken the Area should be a one-shot");
        }
    }
}
