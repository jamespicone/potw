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
    public class GrenadeTests : ParahumanTest
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

            var card = GetCard("Grenade");
            Assert.That(card.IsOneShot, Is.True);
        }

        [Test()]
        public void TestDeals2FireAnd2MeleeDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            DecisionSelectTarget = baron.CharacterCard;

            QuickHPStorage(baron);
            PlayCard("Grenade");
            // 2 fire + 2 melee = 4 total
            QuickHPCheck(-4);
        }

        [Test()]
        public void TestCanDestroyOngoingIfDealtDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var ongoing = PlayCard("LivingForceField");

            DecisionSelectTarget = baron.CharacterCard;
            DecisionSelectCard = ongoing;

            PlayCard("Grenade");

            AssertInTrash(ongoing);
        }

        [Test()]
        public void TestOnlyTargetsNonHeroes()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            DecisionSelectTarget = baron.CharacterCard;

            PlayCard("Grenade");

            // Bunker should not have been selectable
            QuickHPStorage(bunker);
            QuickHPCheck(0);
        }
    }
}
