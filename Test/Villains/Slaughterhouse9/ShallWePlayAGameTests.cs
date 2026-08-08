using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Slaughterhouse9
{
    [TestFixture()]
    public class ShallWePlayAGameTests : Slaughterhouse9TestBase
    {
        [Test()]
        public void TestFetchesJackAndDealsPsychicDamage()
        {
            SetupAndStartNineGame();

            // If Jack wasn't deployed at setup, he's fetched with live triggers and
            // his special-card reaction will play the top card of the villain deck.
            StackDeck("LessThanHuman");

            QuickHPStorage(legacy, bunker, haka);
            AssertDamageType(DamageType.Psychic, DamageType.Psychic, DamageType.Psychic);

            PlayCard("ShallWePlayAGame", 0);

            AssertIsInPlay(jackslash);
            QuickHPCheck(-2, -2, -2);
        }
    }
}
