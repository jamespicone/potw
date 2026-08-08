using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Environment.NewDelhi
{
    [TestFixture()]
    public class AChaoticEnvironmentTests : NewDelhiTestBase
    {
        [Test()]
        public void TestEndOfTurnPlaysTopEnvironmentCard()
        {
            SetupNewDelhiGame();

            var chaotic = PlayCard("AChaoticEnvironment");
            // A second environment card so Chaotic doesn't self-destruct at the
            // start of the environment turn.
            PlayCard("LightningRod");

            var sacrifice = StackDeck("HeroicSacrifice");

            GoToEndOfTurn(env);

            AssertIsInPlay(sacrifice);
            AssertIsInPlay(chaotic);
        }

        [Test()]
        public void TestSelfDestructsWhenAlone()
        {
            SetupNewDelhiGame();

            var chaotic = PlayCard("AChaoticEnvironment");

            GoToStartOfTurn(env);

            AssertInTrash(chaotic);
        }
    }
}
