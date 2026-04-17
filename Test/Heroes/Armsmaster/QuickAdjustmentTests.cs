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
    public class QuickAdjustmentTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();
        }

        [Test()]
        public void TestIsOngoing()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            var card = PlayCard("QuickAdjustment");
            Assert.That(card.DoKeywordsContain("ongoing"), Is.True);
        }

        [Test()]
        public void TestEndOfTurnReturnsModuleToHand()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            var halberd = PlayCard("SurveyHalberd");
            DecisionSelectWord = "Primary";
            var flamethrower = PlayCard("Flamethrower");
            PlayCard("QuickAdjustment");

            // Select flamethrower to return, then decline to play a new module
            DecisionSelectCard = flamethrower;
            DecisionDoNotSelectCard = SelectionType.PlayCard;

            GoToEndOfTurn(armsmaster);

            AssertInHand(flamethrower);
        }

        [Test()]
        public void TestEndOfTurnMayPlayModule()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            var halberd = PlayCard("SurveyHalberd");
            DecisionSelectWord = "Primary";
            var flamethrower = PlayCard("Flamethrower");
            PlayCard("QuickAdjustment");

            // Put a module in hand to play
            var medicator = PutInHand("Medicator");

            // Return flamethrower, then play medicator
            DecisionSelectCards = new Card[] { flamethrower, medicator };

            GoToEndOfTurn(armsmaster);

            AssertInHand(flamethrower);
            AssertIsInPlay(medicator);
        }

        [Test()]
        public void TestEndOfTurnIsOptional()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            var halberd = PlayCard("SurveyHalberd");
            DecisionSelectWord = "Primary";
            var flamethrower = PlayCard("Flamethrower");
            PlayCard("QuickAdjustment");

            // Decline to return any module
            DecisionDoNotSelectCard = SelectionType.ReturnToHand;

            GoToEndOfTurn(armsmaster);

            AssertIsInPlay(flamethrower);
        }

        [Test()]
        public void TestPreventsEquipmentDestruction()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            var halberd = PlayCard("SurveyHalberd");
            var qa = PlayCard("QuickAdjustment");

            // Say yes to protecting the equipment
            DecisionYesNo = true;

            DestroyCard(halberd);

            // Halberd survives, Quick Adjustment is destroyed instead
            AssertIsInPlay(halberd);
            AssertInTrash(qa);
        }

        [Test()]
        public void TestCanDeclineProtection()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            var halberd = PlayCard("SurveyHalberd");
            var qa = PlayCard("QuickAdjustment");

            // Decline to protect
            DecisionYesNo = false;

            DestroyCard(halberd);

            // Halberd destroyed, Quick Adjustment survives
            AssertInTrash(halberd);
            AssertIsInPlay(qa);
        }
    }
}
