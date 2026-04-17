using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Armsmaster
{
    [TestFixture()]
    public class DualWieldingTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();
        }

        [Test()]
        public void TestIsOneShot()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            var card = GetCard("DualWielding");
            Assert.That(card.DoKeywordsContain("one-shot"), Is.True);
        }

        [Test()]
        public void TestUsesHalberdPower()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var halberd = PlayCard("OriginalHalberd");

            DecisionSelectPower = halberd;
            DecisionSelectTarget = baron.CharacterCard;
            DecisionDoNotActivatableAbility = true;

            QuickHPStorage(baron);
            PlayCard("DualWielding");
            // Original Halberd deals 2 melee
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestGoesToTrashAfterPlay()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            var halberd = PlayCard("SurveyHalberd");

            DecisionSelectPower = halberd;
            DecisionDoNotActivatableAbility = true;

            var dw = PlayCard("DualWielding");

            AssertInTrash(dw);
        }
    }
}
