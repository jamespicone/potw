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
    public class NothingIsLostTests : ParahumanTest
    {
        [Test()]
        public void TestEachPlayerRetrievesFromTrash()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Legacy", "Megalopolis");
            StartGame();
            RemoveVillainCards();
            RemoveVillainTriggers();

            var clarity = PutInTrash("Clarity");
            var fortitude = PutInTrash("Fortitude");

            DecisionSelectCards = new Card[] { clarity, fortitude };

            PlayCard("NothingIsLost", 0);

            AssertInHand(clarity);
            AssertInHand(fortitude);
        }
    }
}
