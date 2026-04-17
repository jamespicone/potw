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
    public class EquipmentStashTests : ParahumanTest
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

            var card = GetCard("EquipmentStash");
            Assert.That(card.DoKeywordsContain("one-shot"), Is.True);
        }

        [Test()]
        public void TestSearchDeckForEquipmentIntoPlay()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            var halberd = PutOnDeck("OriginalHalberd");

            DecisionSelectCard = halberd;
            DecisionMoveCardDestination = new MoveCardDestination(armsmaster.TurnTaker.PlayArea);

            PlayCard("EquipmentStash");

            AssertIsInPlay(halberd);
        }

        [Test()]
        public void TestSearchDeckForEquipmentIntoHand()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            var halberd = PutOnDeck("OriginalHalberd");

            DecisionSelectCard = halberd;
            DecisionMoveCardDestination = new MoveCardDestination(armsmaster.HeroTurnTaker.Hand);

            PlayCard("EquipmentStash");

            AssertInHand(halberd);
        }

        [Test()]
        public void TestFindsModules()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            // Modules are equipment too
            var flamethrower = PutOnDeck("Flamethrower");

            // Need a halberd in play for the module to attach to
            PlayCard("SurveyHalberd");

            DecisionSelectCard = flamethrower;
            DecisionMoveCardDestination = new MoveCardDestination(armsmaster.TurnTaker.PlayArea);
            DecisionSelectWord = "Primary";

            PlayCard("EquipmentStash");

            AssertIsInPlay(flamethrower);
        }
    }
}
