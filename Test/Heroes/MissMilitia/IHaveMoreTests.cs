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
    public class IHaveMoreTests : ParahumanTest
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

            var card = GetCard("IHaveMore");
            Assert.That(card.IsOneShot, Is.True);
        }

        [Test()]
        public void TestDestroysWeaponAndOngoings()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            var pistol = PlayCard("Pistol");
            var ongoing1 = PlayCard("LivingForceField");
            var ongoing2 = PlayCard("BacklashField");

            // Pistol auto-selects as the only weapon; then select the two ongoings
            DecisionSelectCards = new Card[] { ongoing1, ongoing2 };

            PlayCard("IHaveMore");

            AssertInTrash(pistol);
            AssertInTrash(ongoing1);
            AssertInTrash(ongoing2);
        }

        [Test()]
        public void TestCanDestroyEnvironmentCards()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            var pistol = PlayCard("Pistol");
            var envCard = PlayCard("ObsidianField");

            // Pistol auto-selects as the only weapon; then select the env card
            DecisionSelectCards = new Card[] { envCard, null };

            PlayCard("IHaveMore");

            AssertInTrash(pistol);
            AssertInTrash(envCard);
        }
    }
}
