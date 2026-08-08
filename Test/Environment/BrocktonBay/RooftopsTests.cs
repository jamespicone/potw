using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Environment.BrocktonBay
{
    [TestFixture()]
    public class RooftopsTests : BrocktonBayTestBase
    {
        [Test()]
        public void TestIncreasesAllDamage()
        {
            SetupBrocktonBayGame();

            PlayCard("Rooftops");

            QuickHPStorage(bunker);
            DealDamage(haka, bunker, 2, DamageType.Melee);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestPlayerMayDiscardTwoToDestroy()
        {
            SetupBrocktonBayGame();

            var rooftops = PlayCard("Rooftops");

            var toDiscard = legacy.HeroTurnTaker.Hand.Cards.Take(2).ToArray();
            DecisionYesNo = true;
            DecisionSelectCards = toDiscard;

            GoToEndOfTurn(legacy);

            AssertInTrash(toDiscard[0]);
            AssertInTrash(toDiscard[1]);
            AssertInTrash(rooftops);
        }

        [Test()]
        public void TestDecliningKeepsRooftopsInPlay()
        {
            SetupBrocktonBayGame();

            var rooftops = PlayCard("Rooftops");

            DecisionYesNo = false;

            GoToEndOfTurn(legacy);

            AssertIsInPlay(rooftops);
        }
    }
}
