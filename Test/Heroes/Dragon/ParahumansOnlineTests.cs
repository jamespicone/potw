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
    public class ParahumansOnlineTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var card = PlayCard("ParahumansOnline");
            AssertIsInPlay(card);
        }

        [Test()]
        public void TestStartOfTurn_PlayersRevealTop()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Bunker", "InsulaPrimalis");
            StartGame();

            var card = PlayCard("ParahumansOnline");

            // Stack decks with known cards
            var dragonCard = PutOnDeck("Analysis");
            var bunkerCard = PutOnDeck("FlakCannon");

            DecisionDoNotActivatableAbility = true;
            DecisionYesNo = true; // Return card to deck

            GoToStartOfTurn(dragon);

            // Cards should have been revealed and returned to deck (based on decision)
        }

        [Test()]
        public void TestFocusAbility_PlayerDraws()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Bunker", "InsulaPrimalis");
            StartGame();

            var card = PlayCard("ParahumansOnline");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            // Activate focus ability - player draws
            DecisionDoNotActivatableAbility = false;
            DecisionActivateAbilities = new Card[] { card };
            DecisionSelectTurnTaker = bunker.TurnTaker;

            QuickHandStorage(bunker);
            UsePower(dragon.CharacterCard);

            QuickHandCheck(1);
        }

        [Test()]
        public void TestIsEquipment()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var card = GetCard("ParahumansOnline");

            Assert.That(card.DoKeywordsContain("equipment"), Is.True, "Parahumans Online should be Equipment");
        }

        [Test()]
        public void TestCostsFocusPoint()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Bunker", "InsulaPrimalis");
            StartGame();

            var tokenPool = dragon.CharacterCard.FindTokenPool("FocusPool");
            var card = PlayCard("ParahumansOnline");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            AssertTokenPoolCount(tokenPool, 4);

            DecisionDoNotActivatableAbility = false;
            DecisionActivateAbilities = new Card[] { card };
            DecisionSelectTurnTaker = bunker.TurnTaker;

            UsePower(dragon.CharacterCard);

            // Power doesn't consume focus - only Focus Phase does
            AssertTokenPoolCount(tokenPool, 4);
        }
    }
}
