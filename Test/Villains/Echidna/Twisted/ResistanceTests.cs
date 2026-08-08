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
    public class ResistanceTests : ParahumanTest
    {
        private void SetupResistanceGame()
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
        public void TestEndOfTurnDamagesLowestHero()
        {
            SetupResistanceGame();

            var resistance = PlayCard("ResistanceTwisted");

            SetHitPoints(alexandria, 25);
            SetHitPoints(bitch, 15);

            QuickHPStorage(alexandria, bitch);
            AssertDamageType(DamageType.Melee);
            AssertDamageSource(resistance);

            GoToEndOfTurn();

            QuickHPCheck(0, -2);
        }

        [Test()]
        public void TestStartOfTurnPlaysTopVillainCard()
        {
            SetupResistanceGame();

            PlayCard("ResistanceTwisted");

            var nightmare = StackDeck("ChimaericalNightmare");

            GoToEndOfTurn();
            GoToStartOfTurn(echidna);

            AssertIsInPlay(nightmare);
        }
    }
}
