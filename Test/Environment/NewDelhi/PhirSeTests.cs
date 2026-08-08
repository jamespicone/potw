using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Environment.NewDelhi
{
    [TestFixture()]
    public class PhirSeTests : NewDelhiTestBase
    {
        [Test()]
        public void TestMovesNextToHighestAndExplodesWhenDestroyed()
        {
            SetupNewDelhiGame();

            var phirSe = PlayCard("PhirSe");

            // Baron Blade (40) is the highest target.
            GoToEndOfTurn(env);
            AssertNextToCard(phirSe, baron.CharacterCard);

            phirSe.FindTokenPool("PhirSePool").SetNumberOfTokens(4);

            QuickHPStorage(baron.CharacterCard);
            AssertDamageType(DamageType.Energy);

            DestroyCard(phirSe);

            QuickHPCheck(-4);
        }

        [Test()]
        public void TestGainsTokensAtStartOfTurn()
        {
            SetupNewDelhiGame();

            var phirSe = PlayCard("PhirSe");

            GoToStartOfTurn(env);

            Assert.That(phirSe.FindTokenPool("PhirSePool").CurrentValue, Is.EqualTo(2));
        }
    }
}
