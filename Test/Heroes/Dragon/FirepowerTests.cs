using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Dragon
{
    [TestFixture()]
    public class FirepowerTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            DecisionSelectCards = new Card[] { dragon.CharacterCard, baron.CharacterCard };
            var card = PlayCard("Firepower");
            AssertInTrash(card); // One-shot goes to trash
        }

        [Test()]
        public void TestDragonDeals3FireDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            QuickHPStorage(baron);
            // Only Dragon is available as damage source (no mechs), so it's auto-selected
            // Only need to specify the damage target
            DecisionSelectTarget = baron.CharacterCard;
            AssertDamageSource(dragon.CharacterCard);
            AssertDamageType(DamageType.Fire);

            PlayCard("Firepower");

            QuickHPCheck(-3);
        }

        [Test()]
        public void TestMechDeals3FireDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var mech = PlayCard("Ladon");

            QuickHPStorage(baron);
            DecisionSelectCards = new Card[] { mech, baron.CharacterCard };
            AssertDamageSource(mech);
            AssertDamageType(DamageType.Fire);

            PlayCard("Firepower");

            QuickHPCheck(-3);
        }

        [Test()]
        public void TestDamageTypeIsFire()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            // Only Dragon is available as damage source (no mechs), so it's auto-selected
            DecisionSelectTarget = baron.CharacterCard;
            AssertDamageType(DamageType.Fire);

            PlayCard("Firepower");
        }

        [Test()]
        public void TestDamageSourceIsSelected()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var mech = PlayCard("Melusine");

            // Select Melusine as damage source
            DecisionSelectCards = new Card[] { mech, baron.CharacterCard };
            AssertDamageSource(mech);

            PlayCard("Firepower");
        }

        [Test()]
        public void TestOnlyDragonOrMechsInPlayArea()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            // Play a mech
            var mech = PlayCard("Ladon");

            // When selecting damage source, only Dragon or Dragon's mechs should be available
            // Bunker should not be selectable
            DecisionSelectCards = new Card[] { dragon.CharacterCard, baron.CharacterCard };

            QuickHPStorage(baron);
            AssertDamageSource(dragon.CharacterCard);

            PlayCard("Firepower");

            QuickHPCheck(-3);
        }

        [Test()]
        public void TestIsOneShot()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var card = GetCard("Firepower");

            Assert.That(card.IsOneShot, Is.True, "Firepower should be a One-Shot");
        }

        [Test()]
        public void TestCanTargetVillain()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            QuickHPStorage(baron);
            // Only Dragon is available as damage source (no mechs), so it's auto-selected
            DecisionSelectTarget = baron.CharacterCard;

            PlayCard("Firepower");

            QuickHPCheck(-3);
        }

        [Test()]
        public void TestCanTargetHero()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Bunker", "InsulaPrimalis");
            StartGame();

            QuickHPStorage(bunker);
            // Only Dragon is available as damage source (no mechs), so it's auto-selected
            DecisionSelectTarget = bunker.CharacterCard;

            PlayCard("Firepower");

            QuickHPCheck(-3);
        }

        [Test()]
        public void TestCanTargetEnvironment()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var envTarget = PlayCard("VelociraptorPack");

            QuickHPStorage(envTarget);
            // Only Dragon is available as damage source (no mechs), so it's auto-selected
            DecisionSelectTarget = envTarget;

            PlayCard("Firepower");

            QuickHPCheck(-3);
        }
    }
}
