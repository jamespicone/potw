using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Dragon
{
    [TestFixture()]
    public class ArchivesTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var card = PlayCard("Archives");
            AssertIsInPlay(card);
        }

        [Test()]
        public void TestFocusAbility_RetrieveOneShot()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var archives = PlayCard("Archives");

            // Put a one-shot in Dragon's trash
            var oneShot = PutInTrash("Analysis");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            // Activate Archives focus ability
            // SelectHeroToMoveCardFromTrash selects hero then card
            DecisionDoNotActivatableAbility = false;
            DecisionActivateAbilities = new Card[] { archives };
            // Only one one-shot in Dragon's trash, card auto-resolves
            DecisionSelectTurnTakers = new TurnTaker[] { dragon.TurnTaker };

            UsePower(dragon.CharacterCard);

            AssertInHand(oneShot);
        }

        [Test()]
        public void TestCanSelectAnyPlayerTrash()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Bunker", "InsulaPrimalis");
            StartGame();

            var archives = PlayCard("Archives");

            // Put a one-shot in Bunker's trash (AdhesiveFoamGrenade is a one-shot)
            var actualOneShot = PutInTrash("AdhesiveFoamGrenade");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            // Activate Archives focus ability - select Bunker's trash
            // SelectHeroToMoveCardFromTrash asks which hero, then which card
            DecisionDoNotActivatableAbility = false;
            DecisionActivateAbilities = new Card[] { archives };
            // Only one one-shot in Bunker's trash, so card selection auto-resolves
            // Just need to select the hero
            DecisionSelectTurnTakers = new TurnTaker[] { bunker.TurnTaker };

            UsePower(dragon.CharacterCard);

            AssertInHand(bunker, actualOneShot);
        }

        [Test()]
        public void TestCostsFocusPoint()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var tokenPool = dragon.CharacterCard.FindTokenPool("FocusPool");
            var archives = PlayCard("Archives");

            // Put a one-shot in trash
            var oneShot = PutInTrash("Analysis");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            AssertTokenPoolCount(tokenPool, 4);

            DecisionDoNotActivatableAbility = false;
            DecisionActivateAbilities = new Card[] { archives };
            DecisionSelectTurnTakers = new TurnTaker[] { dragon.TurnTaker };

            UsePower(dragon.CharacterCard);

            // Power doesn't consume focus - only Focus Phase does
            AssertTokenPoolCount(tokenPool, 4);
        }

        [Test()]
        public void TestIsOngoing()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var card = GetCard("Archives");

            Assert.That(card.IsOngoing, Is.True, "Archives should be an Ongoing");
        }
    }
}
