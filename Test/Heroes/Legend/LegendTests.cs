using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Legend
{
    [TestFixture()]
    public class LegendTests : ParahumanTest
    {
        [Test()]
        public void TestGuiseEffects()
        {
            SetupGameController("BaronBlade", "Guise", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            var kaleidoscope = PlayCard("Kaleidoscope");
            var splitshot = PlayCard("Splitshot");

            GoToPlayCardPhase(guise);

            DecisionSelectTurnTaker = legend.TurnTaker;
            var uyitg = PlayCard("UhYeahImThatGuy");
            ResetDecisions();

            DecisionSelectTargets = new Card[] { baron.CharacterCard, null };
            DecisionSelectDamageType = DamageType.Infernal;

            AssertDamageSource(guise.CharacterCard);
            AssertDamageType(DamageType.Infernal);

            QuickHPStorage(baron);
            UsePower(uyitg);
            QuickHPCheck(-2);
        }

        // TODO: Guise copying Legend's powers crashes when accessing effects.
        // Check CardWithoutReplacements on source or something?
    }
}
