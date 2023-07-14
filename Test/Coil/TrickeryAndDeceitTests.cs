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
    public class TrickeryAndDeceitTests : ParahumanTest
    {
        [Test()]
        public void TestPlay()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Coil",
                "TheWraith",
                "Megalopolis"
            );

            PlayCard("CoilSchemingCharacter"); ;

            var megacomputer = PlayCard("MegaComputer");
            var eyepiece = PlayCard("InfraredEyepiece");
            var stunbolt = PlayCard("StunBolt");

            AssertNumberOfCardsInPlay(c => c.DoKeywordsContain("equipment"), 3);
            PlayCard("TrickeryAndDeceit");
            AssertNumberOfCardsInPlay(c => c.DoKeywordsContain("equipment"), 0);
        }
    }
}
