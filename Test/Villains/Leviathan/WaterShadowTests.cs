using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Leviathan
{
    [TestFixture()]
    public class WaterShadowTests : LeviathanTestBase
    {
        private Card SetupWaterShadow()
        {
            RemoveVillainTriggers();
            return PutTacticInPlay("WaterShadow");
        }

        [Test()]
        public void TestBonusDamageOnLeviathanMelee()
        {
            SetupLeviathanGame();
            SetupWaterShadow();

            QuickHPStorage(haka);
            DealDamage(leviathan.CharacterCard, haka.CharacterCard, 2, DamageType.Melee);

            // 2 melee from Leviathan plus 2 irreducible melee from Water Shadow.
            QuickHPCheck(-4);
        }

        [Test()]
        public void TestBonusDamageIsIrreducible()
        {
            SetupLeviathanGame();
            SetupWaterShadow();

            PlayCard("Fortitude"); // Reduce damage dealt to Legacy by 1

            QuickHPStorage(legacy);
            DealDamage(leviathan.CharacterCard, legacy.CharacterCard, 2, DamageType.Melee);

            // Leviathan's 2 melee is reduced to 1; Water Shadow's 2 is irreducible.
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestNoBonusOnNonMeleeDamage()
        {
            SetupLeviathanGame();
            SetupWaterShadow();

            QuickHPStorage(haka);
            DealDamage(leviathan.CharacterCard, haka.CharacterCard, 2, DamageType.Cold);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestNoBonusOnOtherSourcesMelee()
        {
            SetupLeviathanGame();
            SetupWaterShadow();

            QuickHPStorage(haka);
            DealDamage(bunker, haka, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestIndestructible()
        {
            SetupLeviathanGame();
            var waterShadow = SetupWaterShadow();

            DestroyCard(waterShadow);

            AssertIsInPlay(waterShadow);
        }
    }
}
