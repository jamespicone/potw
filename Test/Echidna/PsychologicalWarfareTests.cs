using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra;

using Jp.ParahumansOfTheWormverse.Echidna;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Echidna
{
    [TestFixture()]
    public class PsychologicalWarfareTests : EchidnaBaseTest
    {
        protected HeroTurnTakerController alexandria { get { return FindHero("Alexandria"); } }
        protected HeroTurnTakerController bitch { get { return FindHero("Bitch"); } }
        protected HeroTurnTakerController legend { get { return FindHero("Legend"); } }

        [Test()]
        public void TestWorks()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "Jp.ParahumansOfTheWormverse.Legend",
                "Megalopolis"
            );

            RemoveAllTwisted();
            PlayCard("PsychologicalWarfare");

            QuickHPStorage(echidna.CharacterCard, alexandria.CharacterCard, bitch.CharacterCard, legend.CharacterCard);
            StartGame();
            QuickHPCheck(0, -2, -2, -2);
        }
    }
}
