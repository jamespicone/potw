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
    public class NotExactlySanitaryTests : ParahumanTest
    {
        [Test()]
        public void TestWorks()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "InsulaPrimalis");

            StartGame();

            PlayCard("NotExactlySanitary");

            QuickHPStorage(tempest);
            DealDamage(merchants, tempest, 1, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestDoesntTriggerToxic()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "InsulaPrimalis");

            StartGame();

            PlayCard("NotExactlySanitary");

            QuickHPStorage(tempest);
            DealDamage(merchants, tempest, 1, DamageType.Toxic);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestNoToxicOnSelfDamage()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "InsulaPrimalis");

            StartGame();

            PlayCard("NotExactlySanitary");

            QuickHPStorage(merchants);
            DealDamage(merchants, merchants, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestNoToxicFromHeroDamage()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "InsulaPrimalis");

            StartGame();

            PlayCard("NotExactlySanitary");

            QuickHPStorage(merchants, tempest);
            DealDamage(tempest, merchants, 2, DamageType.Melee);
            QuickHPCheck(-2, 0);
        }

        [Test()]
        public void TestThugDamageTriggersToxic()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "Legacy", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            PlayCard("NotExactlySanitary");
            PlayCard("Reveller");

            SetHitPoints(tempest, 15);
            SetHitPoints(legacy, 16);
            QuickHPStorage(tempest, legacy);
            GoToEndOfTurn();
            QuickHPCheck(0, -2);
        }

        [Test()]
        public void TestNoToxicWhenDamagePrevented()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "InsulaPrimalis");

            StartGame();

            PlayCard("NotExactlySanitary");
            var mush = PlayCard("Mush");

            QuickHPStorage(mush);
            DealDamage(merchants.CharacterCard, mush, 2, DamageType.Melee);
            QuickHPCheck(0);
        }
    }
}
