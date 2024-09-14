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
    public class MushTests : ParahumanTest
    {
        [Test()]
        public void ImmuneToMelee()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "InsulaPrimalis");

            StartGame();

            var mush = PlayCard("Mush");

            QuickHPStorage(mush);
            DealDamage(tempest, mush, 5, DamageType.Melee);
            QuickHPCheck(0);
            DealDamage(tempest, mush, 5, DamageType.Fire);
            QuickHPCheck(-5);
        }

        [Test()]
        public void ImmuneToProjectile()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "InsulaPrimalis");

            StartGame();

            var mush = PlayCard("Mush");

            QuickHPStorage(mush);
            DealDamage(tempest, mush, 5, DamageType.Projectile);
            QuickHPCheck(0);
            DealDamage(tempest, mush, 5, DamageType.Fire);
            QuickHPCheck(-5);
        }

        [Test()]
        public void DealsDamage()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "Legacy", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            var mush = PlayCard("Mush");

            AssertDamageType(DamageType.Melee);
            AssertDamageSource(mush);
            QuickHPStorage(merchants.CharacterCard, tempest.CharacterCard, legacy.CharacterCard);
            GoToPlayCardPhase(merchants);
            QuickHPCheck(0, 0, 0);
            GoToEndOfTurn(merchants);
            QuickHPCheck(0, -1, -1);
        }
    }
}
