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
    public class AMomentOfReflectionTests : ParahumanTest
    {
        [Test()]
        public void TestOtherPlayerSearchesDeck()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Legacy", "Megalopolis");
            StartGame();
            RemoveVillainCards();
            RemoveVillainTriggers();

            var fortitude = GetCard("Fortitude");
            DecisionSelectCard = fortitude;
            QuickHandStorage(legacy);

            PlayCard("AMomentOfReflection", 0);

            AssertInHand(fortitude);
            QuickHandCheck(1);
        }
    }
}
