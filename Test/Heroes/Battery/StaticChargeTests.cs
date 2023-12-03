using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Battery
{
    [TestFixture()]
    public class StaticChargeTests : ParahumanTest
    {
        [Test()]
        public void TestDischargeUsed()
        {
            SetupGameController("AkashBhuta", "Jp.ParahumansOfTheWormverse.Battery", "InsulaPrimalis");

            StartGame();
            RemoveVillainTriggers();

            var slide = PlayCard("LivingRockslide");
            RemoveCardTriggers(slide);

            PlayCard("StaticCharge");

            UsePower(battery);

            DecisionSelectCards = new Card[] { null };
            UsePower(battery);
            ResetDecisions();

            QuickHPStorage(akash.CharacterCard, slide, battery.CharacterCard);

            AssertDamageSource(battery.CharacterCard);
            AssertDamageType(DamageType.Lightning);

            GoToStartOfTurn(battery);
            QuickHPCheck(-2, -2, 0);
        }

        [Test()]
        public void TestDischargeNotUsed()
        {
            SetupGameController("AkashBhuta", "Jp.ParahumansOfTheWormverse.Battery", "InsulaPrimalis");

            StartGame();
            RemoveVillainTriggers();

            var slide = PlayCard("LivingRockslide");
            RemoveCardTriggers(slide);

            PlayCard("StaticCharge");

            QuickHPStorage(akash.CharacterCard, slide, battery.CharacterCard);

            GoToStartOfTurn(battery);
            QuickHPCheck(0, 0, 0);
        }
    }
}
