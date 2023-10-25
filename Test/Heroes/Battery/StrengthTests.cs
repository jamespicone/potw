using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Battery
{
    [TestFixture()]
    public class StrengthTests : ParahumanTest
    {
        [Test()]
        public void TestUncharged()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Battery", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            DecisionSelectTarget = baron.CharacterCard;

            QuickHPStorage(baron);
            PlayCard("Strength");
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestChargedNormal()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Battery", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            UsePower(battery);

            DecisionSelectTarget = baron.CharacterCard;

            QuickHPStorage(baron);
            PlayCard("Strength");
            QuickHPCheck(-5);
        }

        [Test()]
        public void TestChargedDamagePlus()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Battery", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            UsePower(battery);

            PlayCard("ObsidianField");

            DecisionSelectTarget = baron.CharacterCard;

            QuickHPStorage(baron);
            PlayCard("Strength");
            QuickHPCheck(-7);
        }
    }
}
