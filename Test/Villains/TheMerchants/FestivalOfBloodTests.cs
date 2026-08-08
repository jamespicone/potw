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
    public class FestivalOfBloodTests : ParahumanTest
    {
        [Test()]
        public void TestWorks()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "InsulaPrimalis");

            StartGame();

            PlayCard("FestivalOfBlood");

            AssertSourceDamageModified(new Card[] { merchants.CharacterCard }, new int[] { 1 }, tempest.CharacterCard);
        }

        [Test()]
        public void TestBoostsThugDamage()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "Legacy", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            PlayCard("FestivalOfBlood");
            PlayCard("Reveller");

            SetHitPoints(tempest, 15);
            SetHitPoints(legacy, 16);
            QuickHPStorage(tempest, legacy);
            GoToEndOfTurn();
            QuickHPCheck(0, -2);
        }

        [Test()]
        public void TestDoesNotBoostHeroDamage()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "InsulaPrimalis");

            StartGame();

            PlayCard("FestivalOfBlood");

            AssertSourceDamageModified(new Card[] { tempest.CharacterCard }, new int[] { 0 }, merchants.CharacterCard);
        }

        [Test()]
        public void TestTwoCopiesStack()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "InsulaPrimalis");

            StartGame();

            PlayCard("FestivalOfBlood", 0);
            PlayCard("FestivalOfBlood", 1);

            AssertSourceDamageModified(new Card[] { merchants.CharacterCard }, new int[] { 2 }, tempest.CharacterCard);
        }
    }
}
