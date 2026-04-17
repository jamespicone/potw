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
    public class MacheteTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "InsulaPrimalis");
        }

        [Test()]
        public void TestIsWeaponEquipment()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "InsulaPrimalis");
            StartGame();

            var card = GetCard("Machete");
            Assert.That(card.DoKeywordsContain("weapon"), Is.True);
            Assert.That(card.DoKeywordsContain("equipment"), Is.True);
        }

        [Test()]
        public void TestDeals2MeleeDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var machete = PlayCard("Machete");

            DecisionSelectTarget = baron.CharacterCard;

            QuickHPStorage(baron);
            UsePower(machete);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestSmgEffectDeals3MoreMelee()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var smg = PlayCard("SubmachineGun");
            var machete = PlayCard("Machete");

            // Use SMG power first to activate {smg} effects
            DecisionSelectTarget = baron.CharacterCard;
            UsePower(smg);

            // Now use Machete - {smg} is active, so deals 2 + 3 = 5 melee total
            QuickHPStorage(baron);
            UsePower(machete);
            QuickHPCheck(-5);
        }

        [Test()]
        public void TestNoSmgEffectWithoutSmgActive()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var machete = PlayCard("Machete");

            DecisionSelectTarget = baron.CharacterCard;

            // Without SMG active, only deals 2 melee
            QuickHPStorage(baron);
            UsePower(machete);
            QuickHPCheck(-2);
        }
    }
}
