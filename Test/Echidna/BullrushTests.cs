using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Echidna
{
    [TestFixture()]
    public class BullrushTests : EchidnaBaseTest
    {
        protected HeroTurnTakerController alexandria { get { return FindHero("Alexandria"); } }
        protected HeroTurnTakerController bitch { get { return FindHero("Bitch"); } }

        [Test()]
        public void TestHitsRightTarget()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "Megalopolis"
            );

            StartGame();
            ReturnAllTwisted();

            var decider = AssertNoDecision(SelectionType.LowestHP);
            QuickHPStorage(alexandria.CharacterCard, bitch.CharacterCard);
            PlayCard("Bullrush");
            QuickHPCheck(0, -4);

            SetHitPoints(alexandria, 10);

            QuickHPStorage(alexandria.CharacterCard, bitch.CharacterCard);
            PlayCard("Bullrush");
            QuickHPCheck(-4, 0);
            RestoreOnMakeDecisions(decider);

            SetToSameHitPoints(alexandria, bitch);

            AssertNextDecisionChoices(
                new Card[] { alexandria.CharacterCard, bitch.CharacterCard },
                new Card[] { echidna.CharacterCard }
            );

            DecisionLowestHP = bitch.CharacterCard;
            QuickHPStorage(alexandria.CharacterCard, bitch.CharacterCard);
            PlayCard("Bullrush");
            QuickHPCheck(0, -4);
        }
    }
}
