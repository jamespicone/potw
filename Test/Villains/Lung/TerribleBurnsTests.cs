using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Lung
{
    [TestFixture()]
    public class TerribleBurnsTests : LungTestBase
    {
        [Test()]
        public void TestEndOfTurnDamagesHighestHero()
        {
            SetupLungGame();
            RemoveLungTriggers();

            PlayCard("TerribleBurns");

            SetHitPoints(legacy, 20);
            SetHitPoints(bunker, 15);
            SetHitPoints(haka, 25);

            QuickHPStorage(legacy, bunker, haka);
            AssertDamageSource(lung.CharacterCard);
            AssertDamageType(DamageType.Fire);

            GoToEndOfTurn();

            // H - 2 = 1 fire damage to the highest hero target.
            QuickHPCheck(0, 0, -1);
        }

        [Test()]
        public void TestLungFireDamageDestroysHeroCard()
        {
            SetupLungGame();
            RemoveLungTriggers();

            PlayCard("TerribleBurns");
            var presence = PlayCard("InspiringPresence");

            DecisionSelectCard = presence;
            DealDamage(lung.CharacterCard, haka.CharacterCard, 2, DamageType.Fire);

            AssertInTrash(presence);
        }

        [Test()]
        public void TestLungMeleeDamageDoesNotDestroy()
        {
            SetupLungGame();
            RemoveLungTriggers();

            PlayCard("TerribleBurns");
            var presence = PlayCard("InspiringPresence");

            DecisionSelectCard = presence;
            DealDamage(lung.CharacterCard, haka.CharacterCard, 2, DamageType.Melee);

            AssertIsInPlay(presence);
        }

        [Test()]
        public void TestOtherSourceFireDamageDoesNotDestroy()
        {
            SetupLungGame();
            RemoveLungTriggers();

            PlayCard("TerribleBurns");
            var presence = PlayCard("InspiringPresence");

            DecisionSelectCard = presence;
            DealDamage(bunker.CharacterCard, haka.CharacterCard, 2, DamageType.Fire);

            AssertIsInPlay(presence);
        }
    }
}
