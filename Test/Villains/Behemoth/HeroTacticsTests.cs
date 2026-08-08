using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Behemoth
{
    [TestFixture()]
    public class HeroTacticsTests : BehemothTestBase
    {
        [Test()]
        public void TestIndestructible()
        {
            SetupBehemothGame();

            DestroyCard(heroTactics);

            AssertIsInPlay(heroTactics);
        }

        [Test()]
        public void TestSkipTurnRemovesTwoTokens()
        {
            SetupBehemothGame();
            RemoveCardTriggers(behemoth.CharacterCard);
            ClearProximity();
            SetProximity(legacy, 3);

            DecisionsYesNo = new bool[] { true };
            GoToStartOfTurn(legacy);

            Assert.That(Proximity(legacy).CurrentValue, Is.EqualTo(1));
        }

        [Test()]
        public void TestDeclineSkipTurnKeepsTokens()
        {
            SetupBehemothGame();
            RemoveCardTriggers(behemoth.CharacterCard);
            ClearProximity();
            SetProximity(legacy, 3);

            DecisionYesNo = false;
            GoToStartOfTurn(legacy);

            Assert.That(Proximity(legacy).CurrentValue, Is.EqualTo(3));
        }

        [Test()]
        public void TestEndOfEnvironmentTurnRemoveOneToken()
        {
            SetupBehemothGame();
            RemoveCardTriggers(behemoth.CharacterCard);
            ClearProximity();
            SetProximity(legacy, 2);

            // Third option: one player may remove a proximity token from their hero.
            // Legacy is the only player with tokens, so he's chosen automatically.
            DecisionSelectFunction = 2;
            GoToEndOfTurn(env);

            Assert.That(Proximity(legacy).CurrentValue, Is.EqualTo(1));
        }

        [Test()]
        public void TestFlippedHasNoSkipTurnOption()
        {
            SetupBehemothGame();
            RemoveCardTriggers(behemoth.CharacterCard);
            ClearProximity();
            SetProximity(legacy, 3);

            FlipCard(heroTactics);

            // Panicked side has no skip-turn option; saying yes to everything changes nothing.
            DecisionYesNo = true;
            GoToStartOfTurn(legacy);

            Assert.That(Proximity(legacy).CurrentValue, Is.EqualTo(3));
        }

        [Test()]
        public void TestFlippedPassTwoTokensOption()
        {
            SetupBehemothGame();
            RemoveCardTriggers(behemoth.CharacterCard);
            ClearProximity();
            SetProximity(legacy, 2);

            FlipCard(heroTactics);

            // Second option: one player may move 2 tokens to the next active hero in
            // turn order. Legacy is the only player with tokens; Bunker is after him.
            DecisionSelectFunction = 1;
            GoToEndOfTurn(env);

            Assert.That(Proximity(legacy).CurrentValue, Is.EqualTo(0));
            Assert.That(Proximity(bunker).CurrentValue, Is.EqualTo(2));
        }
    }
}
