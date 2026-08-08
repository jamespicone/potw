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
    public class ClarityTests : ParahumanTest
    {
        [Test()]
        public void TestMayDiscardTopCard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Legacy", "Megalopolis");
            StartGame();
            RemoveVillainCards();
            RemoveVillainTriggers();

            var fortitude = StackDeck(legacy, "Fortitude");

            DecisionYesNo = true;

            PlayCard("Clarity", 0);

            AssertInTrash(fortitude);
        }

        [Test()]
        public void TestMayKeepTopCard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Legacy", "Megalopolis");
            StartGame();
            RemoveVillainCards();
            RemoveVillainTriggers();

            var fortitude = StackDeck(legacy, "Fortitude");

            DecisionYesNo = false;

            PlayCard("Clarity", 0);

            AssertOnTopOfDeck(legacy, fortitude);
        }
    }
}
