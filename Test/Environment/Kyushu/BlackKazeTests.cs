using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Environment.Kyushu
{
    [TestFixture()]
    public class BlackKazeTests : KyushuTestBase
    {
        [Test()]
        public void TestMovesNextToLowestAndAttacks()
        {
            SetupKyushuGame();

            var kaze = PlayCard("BlackKaze");

            SetHitPoints(legacy, 20);
            SetHitPoints(bunker, 10);
            SetHitPoints(haka, 25);

            QuickHPStorage(bunker);
            AssertDamageSource(kaze);
            AssertDamageType(DamageType.Melee);

            GoToEndOfTurn(env);

            // Moves next to the lowest-HP character target, then deals it 3 melee.
            AssertNextToCard(kaze, bunker.CharacterCard);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestDamageIsIrreducible()
        {
            SetupKyushuGame();

            PlayCard("BlackKaze");
            PlayCard("OnlyTheIndomitableRemain"); // reduce all damage by 1

            SetHitPoints(legacy, 20);
            SetHitPoints(bunker, 10);
            SetHitPoints(haka, 25);

            QuickHPStorage(bunker);

            GoToEndOfTurn(env);

            // Kaze's 3 is irreducible; Indomitable's end-of-turn heal nets it back to -2.
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestRedirectsFirstDamageEachRound()
        {
            SetupKyushuGame();

            var kaze = PlayCard("BlackKaze");

            SetHitPoints(legacy, 20);
            SetHitPoints(bunker, 10);
            SetHitPoints(haka, 25);

            // Establish Kaze's victim (Bunker) via the first end-of-turn move.
            GoToEndOfTurn(env);
            AssertNextToCard(kaze, bunker.CharacterCard);

            GoToStartOfTurn(legacy);

            QuickHPStorage(kaze, bunker.CharacterCard);

            // First damage to Kaze this round is redirected to its victim.
            DealDamage(haka, kaze, 4, DamageType.Melee);
            QuickHPCheck(0, -4);

            // Second damage in the same round lands normally.
            DealDamage(haka, kaze, 2, DamageType.Melee);
            QuickHPCheck(-2, 0);
        }
    }
}
