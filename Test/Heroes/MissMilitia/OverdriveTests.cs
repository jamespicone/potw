using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.MissMilitia
{
    [TestFixture()]
    public class OverdriveTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "InsulaPrimalis");
        }

        [Test()]
        public void TestIsOneShot()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "InsulaPrimalis");
            StartGame();

            var card = GetCard("Overdrive");
            Assert.That(card.IsOneShot, Is.True);
        }

        [Test()]
        public void TestDestroysUsedWeaponsAtEndOfTurn()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var pistol = PlayCard("Pistol");
            var machete = PlayCard("Machete");

            GoToPlayCardPhase(missmilitia);
            PlayCard("Overdrive");

            // Use powers on both weapons during power phase
            DecisionSelectTarget = baron.CharacterCard;
            GoToUsePowerPhase(missmilitia);
            UsePower(pistol);
            UsePower(machete);

            // At end of turn, used weapons should be destroyed
            GoToEndOfTurn(missmilitia);

            AssertInTrash(pistol);
            AssertInTrash(machete);
        }
    }
}
