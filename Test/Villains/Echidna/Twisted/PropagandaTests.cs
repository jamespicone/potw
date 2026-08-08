using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Echidna
{
    [TestFixture()]
    public class PropagandaTests : ParahumanTest
    {
        private void SetupPropagandaGame()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "InsulaPrimalis"
            );
            StartGame();
            RemoveVillainTriggers();
            DestroyNonCharacterVillainCards();
            ReturnAllTwisted();
        }

        [Test()]
        public void TestEndOfTurnBaseDamage()
        {
            SetupPropagandaGame();

            var propaganda = PlayCard("PropagandaTwisted");

            SetHitPoints(alexandria, 25);
            SetHitPoints(bitch, 15);

            QuickHPStorage(alexandria, bitch);
            AssertDamageType(DamageType.Energy);
            AssertIrreducible();

            GoToEndOfTurn();

            // X = 2 + 0 Energy tokens.
            QuickHPCheck(-2, 0);
        }

        [Test()]
        public void TestDamageChargesTokensAndClearsThem()
        {
            SetupPropagandaGame();

            var propaganda = PlayCard("PropagandaTwisted");

            // Each hit adds an Energy token.
            DealDamage(alexandria, propaganda, 2, DamageType.Melee);
            DealDamage(alexandria, propaganda, 2, DamageType.Melee);
            Assert.That(propaganda.FindTokenPool("EnergyPool").CurrentValue, Is.EqualTo(2));

            SetHitPoints(alexandria, 25);
            SetHitPoints(bitch, 15);

            QuickHPStorage(alexandria, bitch);

            GoToEndOfTurn();

            // X = 2 + 2 tokens, then the tokens are cleared.
            QuickHPCheck(-4, 0);
            Assert.That(propaganda.FindTokenPool("EnergyPool").CurrentValue, Is.EqualTo(0));
        }
    }
}
