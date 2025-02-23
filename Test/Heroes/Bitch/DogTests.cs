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
    public class DogTests : ParahumanTest
    {
        [Test()]
        public void TestSelfDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "InsulaPrimalis");

            RemoveVillainCards();
            RemoveVillainTriggers();

            StartGame();

            GoToPlayCardPhase(bitch);

            var angelica = PlayCard("Angelica");
            var axel = PlayCard("Axel");
            var bastard = PlayCard("Bastard");
            var brutus = PlayCard("Brutus");
            var bullet = PlayCard("Bullet");
            var ginger = PlayCard("Ginger");
            var judas = PlayCard("Judas");
            var kuro = PlayCard("Kuro");
            var milk = PlayCard("Milk");
            var stumpy = PlayCard("Stumpy");

            QuickHPStorage(angelica, axel, bastard, brutus, bullet, ginger, judas, kuro, milk, stumpy);

            GoToEndOfTurn(bitch);

            QuickHPCheck(-1, -1, 0, -1, -1, -1, -1, -1, -1, -1);
        }
    }
}
