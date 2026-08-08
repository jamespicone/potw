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
    public class PsychologicalTrainingTests : ParahumanTest
    {
        [Test()]
        public void TestOneShotGoesToHand()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Legacy", "Megalopolis");
            StartGame();
            RemoveVillainCards();
            RemoveVillainTriggers();

            GoToPlayCardPhaseAndPlayCard(jessica, "PsychologicalTraining");

            // GoToEndOfTurn skips phase actions entirely (no draw happens), so
            // the stacked card is still on top when the end-of-turn reveal fires.
            var clarity = StackDeck(jessica, "Clarity");

            GoToEndOfTurn(jessica);

            AssertInHand(clarity);
        }

        [Test()]
        public void TestNonOneShotStaysOnDeck()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Legacy", "Megalopolis");
            StartGame();
            RemoveVillainCards();
            RemoveVillainTriggers();

            GoToPlayCardPhaseAndPlayCard(jessica, "PsychologicalTraining");

            var support = StackDeck(jessica, "SupportAndStability");

            GoToEndOfTurn(jessica);

            AssertOnTopOfDeck(jessica, support);
        }
    }
}
