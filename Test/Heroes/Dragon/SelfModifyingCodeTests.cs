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
    public class SelfModifyingCodeTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var card = PutInHand("SelfModifyingCode");
            Assert.That(card, Is.Not.Null);
        }

        [Test()]
        public void TestDeals2IrreduciblePsychicToDragon()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            QuickHPStorage(dragon);
            AssertDamageSource(dragon.CharacterCard);
            AssertDamageType(DamageType.Psychic);
            AssertIrreducible();

            PlayCard("SelfModifyingCode");

            QuickHPCheck(-2);
        }

        [Test()]
        public void TestGains3FocusIfDamageDealt()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var tokenPool = dragon.CharacterCard.FindTokenPool("FocusPool");
            AssertTokenPoolCount(tokenPool, 0);

            PlayCard("SelfModifyingCode");

            // Should have gained 3 focus
            AssertTokenPoolCount(tokenPool, 3);
        }

        [Test()]
        public void TestDamageSourceIsDragon()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            QuickHPStorage(dragon);
            AssertDamageSource(dragon.CharacterCard);

            PlayCard("SelfModifyingCode");

            QuickHPCheck(-2);
        }

        [Test()]
        public void TestDamageTypeIsPsychic()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            AssertDamageType(DamageType.Psychic);

            PlayCard("SelfModifyingCode");
        }

        [Test()]
        public void TestDamageIsIrreducible()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Legacy", "InsulaPrimalis");
            StartGame();

            // Play Lead From The Front to reduce damage to heroes by 1
            PlayCard("LeadFromTheFront");

            QuickHPStorage(dragon);
            AssertIrreducible();

            PlayCard("SelfModifyingCode");

            // Should still deal 2 damage despite damage reduction
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestWorksOutsideDragonTurn()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Bunker", "InsulaPrimalis");
            StartGame();

            var tokenPool = dragon.CharacterCard.FindTokenPool("FocusPool");
            var selfModifyingCode = PutInHand("SelfModifyingCode");

            // Go to Bunker's turn
            GoToPlayCardPhase(bunker);

            QuickHPStorage(dragon);
            PlayCard(selfModifyingCode);
            QuickHPCheck(-2);

            // Should still gain focus
            AssertTokenPoolCount(tokenPool, 3);
        }

        [Test()]
        public void TestIsOneShot()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var card = GetCard("SelfModifyingCode");

            Assert.That(card.IsOneShot, Is.True, "Self-Modifying Code should be a One-Shot");
        }

        [Test()]
        public void TestMultipleCopiesStackFocus()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var tokenPool = dragon.CharacterCard.FindTokenPool("FocusPool");
            AssertTokenPoolCount(tokenPool, 0);

            PlayCard("SelfModifyingCode", 0);
            AssertTokenPoolCount(tokenPool, 3);

            PlayCard("SelfModifyingCode", 1);
            AssertTokenPoolCount(tokenPool, 6);
        }
    }
}
