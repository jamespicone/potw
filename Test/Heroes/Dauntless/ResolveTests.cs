using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Dauntless
{
    [TestFixture()]
    public class ResolveTests : ParahumanTest
    {
        [Test()]
        public void TestSearchDeckForCharge()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            // Stack deck with a charge card
            var crystal = GetCard("Crystallization");
            StackDeck(crystal);

            DecisionSelectLocation = new LocationChoice(dauntless.TurnTaker.Deck);
            DecisionMoveCardDestination = new MoveCardDestination(dauntless.TurnTaker.PlayArea);
            DecisionSelectCard = dauntless.CharacterCard;
            DecisionSelectTarget = baron.CharacterCard;

            PlayCard("Resolve");

            // Crystallization should be in play, next to Dauntless
            AssertIsInPlay(crystal);
        }

        [Test()]
        public void TestSearchTrashForCharge()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            // Put a charge card in trash
            var crystal = GetCard("Crystallization");
            PutInTrash(crystal);

            DecisionSelectLocation = new LocationChoice(dauntless.TurnTaker.Trash);
            DecisionMoveCardDestination = new MoveCardDestination(dauntless.TurnTaker.PlayArea);
            DecisionSelectCard = dauntless.CharacterCard;
            DecisionSelectTarget = baron.CharacterCard;

            PlayCard("Resolve");

            // Crystallization should be in play
            AssertIsInPlay(crystal);
        }

        [Test()]
        public void TestCanPutIntoHand()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            var crystal = GetCard("Crystallization");
            StackDeck(crystal);

            DecisionSelectLocation = new LocationChoice(dauntless.TurnTaker.Deck);
            DecisionMoveCardDestination = new MoveCardDestination(dauntless.HeroTurnTaker.Hand);

            PlayCard("Resolve");

            // Crystallization should be in hand
            AssertInHand(crystal);
        }

        [Test()]
        public void TestFindsPlasmaCoreAsCharge()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            var plasmaCore = GetCard("PlasmaCore");
            StackDeck(plasmaCore);

            DecisionSelectLocation = new LocationChoice(dauntless.TurnTaker.Deck);
            DecisionMoveCardDestination = new MoveCardDestination(dauntless.TurnTaker.PlayArea);
            DecisionSelectCard = dauntless.CharacterCard;

            PlayCard("Resolve");

            // Plasma Core should be in play (it has Charge keyword)
            AssertIsInPlay(plasmaCore);
        }

        [Test()]
        public void TestFindsMatterToEnergyAsCharge()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            var mte = GetCard("MatterToEnergy");
            StackDeck(mte);

            DecisionSelectLocation = new LocationChoice(dauntless.TurnTaker.Deck);
            DecisionMoveCardDestination = new MoveCardDestination(dauntless.TurnTaker.PlayArea);
            DecisionSelectCard = dauntless.CharacterCard;

            PlayCard("Resolve");

            // Matter To Energy should be in play (it has Charge keyword)
            AssertIsInPlay(mte);
        }

        [Test()]
        public void TestNoChargeInDeck()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            // Remove all charge cards from deck
            var charges = FindCardsWhere(c => c.DoKeywordsContain("charge") && c.Location == dauntless.TurnTaker.Deck);
            foreach (var charge in charges)
            {
                MoveCard(dauntless, charge, dauntless.TurnTaker.OutOfGame);
            }

            DecisionSelectLocation = new LocationChoice(dauntless.TurnTaker.Deck);

            // Should still work, just no card found
            PlayCard("Resolve");
        }

        [Test()]
        public void TestShufflesAfterSearchingDeck()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            // Put Plasma Core in deck (a charge that doesn't deal damage on enter play)
            var plasmaCore = GetCard("PlasmaCore");
            MoveCard(dauntless, plasmaCore, dauntless.TurnTaker.Deck);

            DecisionSelectLocation = new LocationChoice(dauntless.TurnTaker.Deck);
            DecisionSelectCard = dauntless.CharacterCard;

            PlayCard("Resolve");

            // PlasmaCore should be in play
            AssertIsInPlay(plasmaCore);
        }
    }
}
