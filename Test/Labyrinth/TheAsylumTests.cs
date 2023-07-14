using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Jp.ParahumansOfTheWormverse.Labyrinth;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Labyrinth
{
    [TestFixture()]
    public class TheAsylumTests : BaseTest
    {
        protected HeroTurnTakerController labyrinth { get { return FindHero("Labyrinth"); } }
        protected HeroTurnTakerController tattletale { get { return FindHero("Tattletale"); } }

        [Test()]
        public void TestDoesDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "InsulaPrimalis");

            StartGame();
            GoToUsePowerPhase(labyrinth);

            RemoveVillainCards();
            var battalion = PlayCard("BladeBattalion");

            PlayCard("ObsidianField");
            PlayCard("TheAsylum");

            QuickHPStorage(labyrinth.CharacterCard, baron.CharacterCard, battalion);
            GoToEndOfTurn(labyrinth);            
            QuickHPCheck(-3, -2, -2);
        }
    }
}
