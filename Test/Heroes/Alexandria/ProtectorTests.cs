using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Alexandria
{
    [TestFixture()]
    public class ProtectorTests : ParahumanTest
    {
        [Test()]
        public void TestHeals()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "InsulaPrimalis");

            RemoveVillainCards();
            RemoveVillainTriggers();

            StartGame();

            SetHitPoints(alexandria, 10);

            QuickHPStorage(alexandria);
            PlayCard("Protector");
            QuickHPCheck(2);
        }

        [Test()]
        public void TestRedirect()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "Legacy", "Tempest", "InsulaPrimalis");

            RemoveVillainCards();
            RemoveVillainTriggers();

            StartGame();

            GoToPlayCardPhase(alexandria);
            PlayCard("Protector");

            AssertNumberOfStatusEffectsInPlay(0);
            DecisionSelectCard = legacy.CharacterCard;
            GoToEndOfTurn(alexandria);
            AssertNumberOfStatusEffectsInPlay(1);

            ResetDecisions();

            QuickHPStorage(alexandria, tempest, legacy);
            DealDamage(legacy, legacy, 2, DamageType.Melee);
            QuickHPCheck(-2, 0, 0);
            DealDamage(legacy, legacy, 2, DamageType.Melee);
            QuickHPCheck(-2, 0, 0);
            DealDamage(legacy, tempest, 2, DamageType.Melee);
            QuickHPCheck(0, -2, 0);

            GoToEndOfTurn(baron);
            AssertNumberOfStatusEffectsInPlay(1);
            QuickHPStorage(alexandria, tempest, legacy);
            DealDamage(legacy, legacy, 2, DamageType.Melee);
            QuickHPCheck(-2, 0, 0);
            DealDamage(legacy, legacy, 2, DamageType.Melee);
            QuickHPCheck(-2, 0, 0);
            DealDamage(legacy, tempest, 2, DamageType.Melee);
            QuickHPCheck(0, -2, 0);

            GoToStartOfTurn(alexandria);
            AssertNumberOfStatusEffectsInPlay(0);
            QuickHPStorage(alexandria, tempest, legacy);
            DealDamage(legacy, legacy, 2, DamageType.Melee);
            QuickHPCheck(0, 0, -2);
        }
    }
}
