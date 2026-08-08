using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.JessicaYamada
{
    [TestFixture()]
    public class HelpNoMatterTheCostTests : ParahumanTest
    {
        [Test()]
        public void TestDrawsWithoutDiscards()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Legacy", "Megalopolis");
            StartGame();
            RemoveVillainCards();
            RemoveVillainTriggers();

            QuickHandStorage(legacy);
            DecisionDoNotSelectCard = SelectionType.DiscardCard;

            PlayCard("HelpNoMatterTheCost", 0);

            // X = 0 discards + 2.
            QuickHandCheck(2);
        }

        [Test()]
        public void TestDiscardsIncreaseDraws()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Legacy", "Megalopolis");
            StartGame();
            RemoveVillainCards();
            RemoveVillainTriggers();

            var hand = jessica.HeroTurnTaker.Hand.Cards.Take(2).ToArray();

            QuickHandStorage(jessica, legacy);
            DecisionSelectCards = new Card[] { hand[0], hand[1], null };

            PlayCard("HelpNoMatterTheCost", 0);

            // Jessica discarded 2; Legacy draws 2 + 2.
            QuickHandCheck(-2, 4);
        }
    }
}
