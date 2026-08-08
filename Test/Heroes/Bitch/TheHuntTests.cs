using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Bitch
{
    [TestFixture()]
    public class TheHuntTests : ParahumanTest
    {
        [Test()]
        public void TestDogsAttackAtStartOfTurn()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "InsulaPrimalis");
            StartGame();
            RemoveVillainCards();
            RemoveVillainTriggers();

            PlayCard("TheHunt");
            var brutus = PlayCard("Brutus");

            QuickHPStorage(baron);
            AssertDamageType(DamageType.Melee);
            AssertDamageSource(brutus);
            DecisionSelectTarget = baron.CharacterCard;

            GoToStartOfTurn(bitch);

            QuickHPCheck(-1);
        }
    }
}
