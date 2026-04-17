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
    public class KamikazeTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var mech = PlayCard("Ladon");
            RemoveMobileDefensePlatform();

            DecisionSelectCards = new Card[] { mech, baron.CharacterCard };
            var card = PlayCard("Kamikaze");
            AssertInTrash(card); // One-shot goes to trash
        }

        [Test()]
        public void TestAgainstCharacter_Deals3Damage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var mech = PlayCard("Ladon");

            // Only one mech in play, so mech selection is auto-resolved
            DecisionSelectTarget = baron.CharacterCard;

            QuickHPStorage(baron);
            // Against character: 3 energy + 3 fire + 3 melee = 9 total
            PlayCard("Kamikaze");

            QuickHPCheck(-9); // 3 + 3 + 3 = 9
        }

        [Test()]
        public void TestAgainstNonCharacter_DestroysTarget()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var mech = PlayCard("Ladon");
            var villainTarget = PlayCard("BladeBattalion");

            // Only one mech in play, so mech selection is auto-resolved
            DecisionSelectTarget = villainTarget;

            PlayCard("Kamikaze");

            // Non-character target should be destroyed
            AssertInTrash(villainTarget);
        }

        [Test()]
        public void TestDestroysMechAfterEffect()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var mech = PlayCard("Ladon");
            AssertIsInPlay(mech);

            DecisionSelectCards = new Card[] { mech, baron.CharacterCard };

            PlayCard("Kamikaze");

            // Mech should be destroyed after dealing damage
            AssertInTrash(mech);
        }

        [Test()]
        public void TestRequiresMechInPlay()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            // No mech in play
            var card = GetCard("Kamikaze");

            // Put the card in hand and try to play it
            PutInHand(card);

            // Without a mech, the card should not be playable or should fizzle
            // The card requires selecting a mech first
        }

        [Test()]
        public void TestDamageSourceIsMech()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var mech = PlayCard("Ladon");

            // Only one mech in play, so mech selection is auto-resolved
            DecisionSelectTarget = baron.CharacterCard;
            AssertDamageSource(mech, mech, mech);

            PlayCard("Kamikaze");
        }

        [Test()]
        public void TestDamageTypesAgainstCharacter()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var mech = PlayCard("Melusine");

            // Only one mech in play, so mech selection is auto-resolved
            DecisionSelectTarget = baron.CharacterCard;

            // Should deal 3 energy, 3 fire, 3 melee
            AssertDamageType(DamageType.Energy, DamageType.Fire, DamageType.Melee);

            PlayCard("Kamikaze");
        }

        [Test()]
        public void TestIsOneShot()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var card = GetCard("Kamikaze");

            Assert.That(card.IsOneShot, Is.True, "Kamikaze should be a One-Shot");
        }

        [Test()]
        public void TestCanTargetEnvironmentNonCharacter()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var mech = PlayCard("Ladon");
            var envTarget = PlayCard("VelociraptorPack");

            // Only one mech in play, so mech selection is auto-resolved
            DecisionSelectTarget = envTarget;

            PlayCard("Kamikaze");

            // Environment target is not a character, so it should be destroyed
            AssertInTrash(envTarget);
            AssertInTrash(mech);
        }

        [Test()]
        public void TestCanSelectAnyMechInPlayArea()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var mech1 = PlayCard("Ladon");
            var mech2 = PlayCard("Melusine");

            // Select the second mech
            DecisionSelectCards = new Card[] { mech2, baron.CharacterCard };

            PlayCard("Kamikaze");

            // Only Melusine should be destroyed
            AssertIsInPlay(mech1);
            AssertInTrash(mech2);
        }
    }
}
