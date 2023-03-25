using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Jp.ParahumansOfTheWormverse.Tattletale;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Coil
{
    [TestFixture()]
    public class IKnowAllYourTricksTests : BaseTest
    {
        [Test()]
        public void TestPlay()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Coil",
                "Ra",
                "Megalopolis"
            );

            PlayCard("CoilSchemingCharacter");

            var megacomputer = PlayCard("FlameBarrier");
            var eyepiece = PlayCard("FleshOfTheSunGod");
            var stunbolt = PlayCard("ImbuedFire");
            
            AssertNumberOfCardsInPlay(c => c.IsOngoing, 3);
            PlayCard("IKnowAllYourTricks");
            AssertNumberOfCardsInPlay(c => c.IsOngoing, 0);
        }
    }
}
