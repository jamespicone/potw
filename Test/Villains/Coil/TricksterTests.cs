using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Coil
{
    [TestFixture()]
    public class TricksterTests : CoilTestBase
    {
        [Test()]
        public void TestFirstDamageEachRoundRedirected()
        {
            SetupCoilGame();
            RemoveCoilTriggers();
            CleanupSetupNoise();

            var trickster = PlayCard("Trickster");

            SetHitPoints(legacy, 20);
            SetHitPoints(bunker, 10);
            SetHitPoints(haka, 25);

            QuickHPStorage(trickster, bunker.CharacterCard);

            // First damage redirected to the lowest hero target.
            DealDamage(haka, trickster, 3, DamageType.Melee);
            QuickHPCheck(0, -3);

            // Second damage in the same round sticks.
            DealDamage(haka, trickster, 3, DamageType.Melee);
            QuickHPCheck(-3, 0);
        }

        [Test()]
        public void TestEndOfTurnDestroysCardOfBusiestPlayer()
        {
            SetupCoilGame();
            RemoveCoilTriggers();
            CleanupSetupNoise();

            PlayCard("Trickster");

            // Legacy has the most cards in play.
            var sense = PlayCard("DangerSense");
            PlayCard("InspiringPresence");

            DecisionSelectCard = sense;

            GoToEndOfTurn();

            AssertInTrash(sense);
        }
    }
}
