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
    public class DragonTests : ParahumanTest
    {
        [Test()]
        public void TestPhases()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            var tokenPool = dragon.CharacterCard.FindTokenPool("FocusPool");

            DecisionDoNotActivatableAbility = true;

            GoToStartOfTurn(dragon);
            AssertCurrentTurnPhase(dragon, Phase.Start);
            AssertTokenPoolCount(tokenPool, 4);

            EnterNextTurnPhase();
            AssertCurrentTurnPhase(dragon, Phase.Unknown);
            AssertTokenPoolCount(tokenPool, 4);

            EnterNextTurnPhase();
            AssertCurrentTurnPhase(dragon, Phase.End);
            AssertTokenPoolCount(tokenPool, 0);
        }

        [Test()]
        public void TestRepOfEarth()
        {
            SetupGameController("BaronBlade", "Legacy", "TheCelestialTribunal");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            SelectFromBoxForNextDecision("Jp.ParahumansOfTheWormverse.DragonCharacter", "Jp.ParahumansOfTheWormverse.Dragon");
            var rep = PlayCard("RepresentativeOfEarth");
            var dragonCard = rep.NextToLocation.Cards.FirstOrDefault();
            Assert.That(dragonCard, Is.Not.Null);
            Assert.That(dragonCard.Identifier, Is.EqualTo("DragonCharacter"));

            var tokenPool = dragonCard.FindTokenPool("FocusPool");

            StackDeck("CalledToJudgement");

            GoToStartOfTurn(env);
            AssertCurrentTurnPhase(env, Phase.Start);
            AssertTokenPoolCount(tokenPool, 4);
            EnterNextTurnPhase();

            AssertCurrentTurnPhase(env, Phase.PlayCard);
            AssertTokenPoolCount(tokenPool, 4);
            EnterNextTurnPhase();

            AssertCurrentTurnPhase(env, Phase.End);
            AssertTokenPoolCount(tokenPool, 0);
            EnterNextTurnPhase();
        }

        // Crashes if it's in the environment via representative of earth
    }
}
