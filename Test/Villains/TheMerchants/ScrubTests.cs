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
    public class ScrubTests : ParahumanTest
    {
        [Test()]
        public void TestDamagesMultipleOfThree()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "Legacy", "Parse", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            GoToEndOfTurn(env);

            MoveAllCards(env, env.TurnTaker.PlayArea, env.TurnTaker.Deck);

            var scrub = PlayCard("Scrub");
            var raptor = PlayCard("VelociraptorPack");
            var trex = PlayCard("EnragedTRex");

            SetHitPoints(raptor, 4);
            SetHitPoints(trex, 9);
            SetHitPoints(legacy, 21);
            SetHitPoints(parse, 15);
            SetHitPoints(tempest, 16);
            SetHitPoints(merchants.CharacterCard, 12);
            SetHitPoints(scrub, 9);

            QuickHPStorage(tempest.CharacterCard, legacy.CharacterCard, parse.CharacterCard, merchants.CharacterCard, raptor, trex, scrub);
            AssertDamageSource(scrub);
            AssertDamageType(DamageType.Energy);
            GoToStartOfTurn(merchants);
            QuickHPCheck(
                0,
                -5,
                -5,
                -5,
                0,
                -5,
                0
            );
        }
    }
}