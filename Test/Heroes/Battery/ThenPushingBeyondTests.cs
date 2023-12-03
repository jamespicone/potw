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
    public class ThenPushingBeyondTests : ParahumanTest
    {
        [Test()]
        public void TestPushingBeyond()
        {
            SetupGameController("AkashBhuta", "Jp.ParahumansOfTheWormverse.Battery", "InsulaPrimalis");

            StartGame();

            MoveAllCardsFromHandToDeck(battery);
            var magnetism = GetCard("Magnetism");
            var pushing = GetCard("ThenPushingBeyond");

            MoveCard(battery, magnetism, battery.HeroTurnTaker.Hand);
            MoveCard(battery, pushing, battery.HeroTurnTaker.Hand);

            DecisionSelectCards = new Card[] { magnetism, null };
            DecisionSelectPower = magnetism;

            AssertDamageSource(battery.CharacterCard);
            AssertDamageType(DamageType.Psychic);

            QuickHandStorage(battery);
            QuickHPStorage(battery.CharacterCard);
            PlayCard(pushing);
            // Battery -1 played pushing, -1 played magnetism
            QuickHandCheck(-2);
            QuickHPCheck(-3);

            AssertIsInPlay(magnetism);
            AssertNotUsablePower(battery, magnetism);
        }
    }
}
