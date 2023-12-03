using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Battery
{
    [TestFixture()]
    public class MagnetismTests : ParahumanTest
    {
        [Test()]
        public void TestUncharged()
        {
            SetupGameController("AkashBhuta", "Jp.ParahumansOfTheWormverse.Battery", "InsulaPrimalis");

            StartGame();

            var threads = MoveCard(battery, "GlowingThreads", battery.TurnTaker.Trash);

            var magnetism = PlayCard("Magnetism");
            
            DecisionSelectCard = threads;
            UsePower(magnetism);
            AssertIsInPlay(threads);
        }

        [Test()]
        public void TestUnchargedAnotherHero()
        {
            SetupGameController("AkashBhuta", "Jp.ParahumansOfTheWormverse.Battery", "Legacy", "InsulaPrimalis");

            StartGame();

            var ring = MoveCard(legacy, "TheLegacyRing", legacy.TurnTaker.Trash);

            var magnetism = PlayCard("Magnetism");

            DecisionSelectCard = ring;
            UsePower(magnetism);
            AssertIsInPlay(ring);
        }

        [Test()]
        public void TestChargedDevice()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Battery", "InsulaPrimalis");

            StartGame();

            var magnetism = PlayCard("Magnetism");
            var platform = GetMobileDefensePlatform();
            UsePower(battery.CharacterCard);

            DecisionSelectCard = platform.Card;
            UsePower(magnetism);
            AssertInTrash(platform.Card);
        }

        [Test()]
        public void TestChargedCantTargetCharacterDevice()
        {
            SetupGameController("Omnitron", "Jp.ParahumansOfTheWormverse.Battery", "InsulaPrimalis");

            StartGame();

            var magnetism = PlayCard("Magnetism");
            UsePower(battery.CharacterCard);

            AssertNextDecisionChoices(notIncluded: new Card[] { omnitron.CharacterCard });

            UsePower(magnetism);
            AssertIsInPlay(omnitron.CharacterCard);
        }

        [Test()]
        public void TestChargedEquipment()
        {
            SetupGameController("AkashBhuta", "Jp.ParahumansOfTheWormverse.Battery", "InsulaPrimalis");

            StartGame();

            var magnetism = PlayCard("Magnetism");
            var threads = PlayCard("GlowingThreads");

            UsePower(battery.CharacterCard);

            DecisionSelectCard = threads;
            UsePower(magnetism);
            AssertInTrash(threads);
        }
    }
}
