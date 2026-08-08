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
    public class ImpossibleEnduranceTests : LeviathanTestBase
    {
        [Test()]
        public void TestPlaysOneShotFromTrashAtStartOfTurn()
        {
            SetupLeviathanGame();
            RemoveVillainTriggers();
            MoveTacticsToDeckBottom();
            RemoveEnvironmentDeck();

            PlayCard("ImpossibleEndurance");

            // Trash: one one-shot (Whipping Tail) and one ongoing (Impossible Strength).
            var whip = PutInTrash("WhippingTail");
            var strength = PutInTrash("ImpossibleStrength");

            SetHitPoints(legacy, 15);
            SetHitPoints(bunker, 16);
            SetHitPoints(haka, 30);

            QuickHPStorage(legacy, bunker, haka);

            GoToStartOfTurn(leviathan);

            // Whipping Tail was played: 2 melee to the 2 lowest hero targets.
            QuickHPCheck(-2, -2, 0);

            // The one-shot ends up back in the trash; the ongoing was never played.
            AssertInTrash(whip);
            AssertInTrash(strength);
        }
    }
}
