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
    public class SecurityFeedsTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var card = PlayCard("SecurityFeeds");
            AssertIsInPlay(card);
        }

        [Test()]
        public void TestFocusAbility_RevealEnvironmentTop()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var card = PlayCard("SecurityFeeds");

            // Stack environment deck
            var envCard = PutOnDeck("VelociraptorPack");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            // Activate focus ability - reveal environment top
            DecisionDoNotActivatableAbility = false;
            DecisionActivateAbilities = new Card[] { card };
            // RevealTopCard_PutItBackOrDiscardIt asks "Discard?" - No means return to deck
            DecisionYesNo = false;

            UsePower(dragon.CharacterCard);

            // Card should have been revealed and returned
            AssertOnTopOfDeck(envCard);
        }

        [Test()]
        public void TestCanReturnOrDiscard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var card = PlayCard("SecurityFeeds");

            // Stack environment deck
            var envCard = PutOnDeck("VelociraptorPack");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            // Activate focus ability - discard the card
            DecisionDoNotActivatableAbility = false;
            DecisionActivateAbilities = new Card[] { card };
            // RevealTopCard_PutItBackOrDiscardIt asks "Discard?" - Yes means discard
            DecisionYesNo = true;

            UsePower(dragon.CharacterCard);

            AssertInTrash(envCard);
        }

        [Test()]
        public void TestCostsFocusPoint()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var tokenPool = dragon.CharacterCard.FindTokenPool("FocusPool");
            var card = PlayCard("SecurityFeeds");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            AssertTokenPoolCount(tokenPool, 4);

            DecisionDoNotActivatableAbility = false;
            DecisionActivateAbilities = new Card[] { card };
            DecisionYesNo = false; // No means return to deck

            UsePower(dragon.CharacterCard);

            // Power doesn't consume focus - only Focus Phase does
            AssertTokenPoolCount(tokenPool, 4);
        }

        [Test()]
        public void TestIsOngoing()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var card = GetCard("SecurityFeeds");

            Assert.That(card.IsOngoing, Is.True, "Security Feeds should be an Ongoing");
        }
    }
}
