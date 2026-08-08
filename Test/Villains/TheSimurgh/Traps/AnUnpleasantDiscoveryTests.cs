using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.TheSimurgh
{
    [TestFixture()]
    public class AnUnpleasantDiscoveryTests : SimurghTestBase
    {
        [Test()]
        public void TestDiscardedHeroCardCausesPsychicDamage()
        {
            SetupSimurghGame();
            RemoveSimurghTriggers();

            FlipTrapFaceUpDormant("AnUnpleasantDiscovery");

            QuickHPStorage(legacy, bunker, haka);
            AssertDamageType(DamageType.Psychic);

            DiscardTopCards(legacy.TurnTaker, 1);

            QuickHPCheck(-1, 0, 0);
        }
    }
}
