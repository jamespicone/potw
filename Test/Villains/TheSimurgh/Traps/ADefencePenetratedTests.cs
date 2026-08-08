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
    public class ADefencePenetratedTests : SimurghTestBase
    {
        [Test()]
        public void TestHeroDamageCannotBeRedirected()
        {
            SetupSimurghGame();
            RemoveSimurghTriggers();

            FlipTrapFaceUpDormant("ADefencePenetrated");

            // Lead From The Front would let Legacy redirect villain damage from
            // Bunker to himself, but the trap prevents it.
            PlayCard("LeadFromTheFront");
            DecisionYesNo = true;

            QuickHPStorage(legacy, bunker);

            DealDamage(simurgh.CharacterCard, bunker.CharacterCard, 2, DamageType.Projectile);

            QuickHPCheck(0, -2);
        }
    }
}
