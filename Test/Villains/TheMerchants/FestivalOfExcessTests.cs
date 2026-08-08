using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.TheMerchants
{
    [TestFixture()]
    public class FestivalOfExcessTests : ParahumanTest
    {
        [Test()]
        public void TestWorks()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "InsulaPrimalis");

            StartGame();

            PlayCard("FestivalOfExcess");

            AssertSourceDamageModified(new Card[] { tempest.CharacterCard }, new int[] { -1 }, merchants.CharacterCard);
        }

        [Test()]
        public void TestReducesDamageToThugs()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            PlayCard("FestivalOfExcess");
            var reveller = PlayCard("Reveller");

            QuickHPStorage(reveller);
            DealDamage(tempest, reveller, 2, DamageType.Fire);
            QuickHPCheck(-1);
            AssertIsInPlay(reveller);
        }

        [Test()]
        public void TestDoesNotReduceDamageToHeroes()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "InsulaPrimalis");

            StartGame();

            PlayCard("FestivalOfExcess");

            QuickHPStorage(tempest);
            DealDamage(merchants, tempest, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestTwoCopiesStack()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "InsulaPrimalis");

            StartGame();

            PlayCard("FestivalOfExcess", 0);
            PlayCard("FestivalOfExcess", 1);

            AssertSourceDamageModified(new Card[] { tempest.CharacterCard }, new int[] { -2 }, merchants.CharacterCard);
        }
    }
}
