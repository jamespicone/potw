using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Tattletale
{
    [TestFixture()]
    public class DominoMaskTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();
        }

        [Test()]
        public void TestIsEquipmentLimited()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            var card = GetCard("DominoMask");
            Assert.That(card.DoKeywordsContain("equipment"), Is.True);
            Assert.That(card.DoKeywordsContain("limited"), Is.True);
        }

        [Test()]
        public void TestGrantsExtraPowerUse()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Tattletale", "Bunker", "InsulaPrimalis");
            StartGame();

            PlayCard("DominoMask");

            // Play two cards with powers
            SetHitPoints(tattletale, 20);
            SetHitPoints(bunker, 20);
            var eye1 = PlayCard("MyEyeOnYou", 0);
            var eye2 = PlayCard("MyEyeOnYou", 1);

            DecisionSelectCard = tattletale.CharacterCard;

            GoToUsePowerPhase(tattletale);

            QuickHPStorage(tattletale);
            UsePower(eye1);
            UsePower(eye2);
            // Both powers should succeed, healing 3 HP each = +6
            QuickHPCheck(6);
        }
    }
}
