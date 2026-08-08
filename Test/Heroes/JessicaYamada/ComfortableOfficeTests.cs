using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.JessicaYamada
{
    [TestFixture()]
    public class ComfortableOfficeTests : ParahumanTest
    {
        [Test()]
        public void TestHealsHeroAtStartOfTurn()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Legacy", "Megalopolis");
            StartGame();
            RemoveVillainCards();
            RemoveVillainTriggers();

            PlayCard("ComfortableOffice");

            SetHitPoints(legacy, 20);
            DecisionSelectCard = legacy.CharacterCard;

            GoToStartOfTurn(jessica);

            AssertHitPoints(legacy, 23);
        }
    }
}
