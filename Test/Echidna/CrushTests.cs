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
    public class CrushTests : EchidnaBaseTest
    {
        protected HeroTurnTakerController alexandria { get { return FindHero("Alexandria"); } }
        protected HeroTurnTakerController bitch { get { return FindHero("Bitch"); } }
        protected HeroTurnTakerController legend { get { return FindHero("Legend"); } }

        [Test()]
        public void TestHitsRightTarget()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "Jp.ParahumansOfTheWormverse.Legend",
                "Megalopolis"
            );

            StartGame();
            RemoveAllTwisted();

            DecisionSelectCards = new Card[] { alexandria.CharacterCard, bitch.CharacterCard, legend.CharacterCard };
            PlayCard("Engulfed", 0);
            PlayCard("Engulfed", 1);
            PlayCard("Engulfed", 2);
            ResetDecisions();

            var legendEngulfed = FindCard(c => c.IsAnEngulfedCard() && c.Location == legend.CharacterCard.NextToLocation);
            MoveCard(echidna, legendEngulfed, echidna.CharacterCard.NextToLocation);

            QuickHPStorage(echidna.CharacterCard, alexandria.CharacterCard, bitch.CharacterCard, legend.CharacterCard);

            PlayCard("Crush");

            QuickHPCheck(0, -3, -3, 0);
        }
    }
}
