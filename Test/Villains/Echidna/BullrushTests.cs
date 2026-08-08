using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

using Jp.ParahumansOfTheWormverse.Echidna;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Echidna
{
    [TestFixture()]
    public class BullrushTests : ParahumanTest
    {
        [Test()]
        public void TestHitsRightTarget()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "Megalopolis"
            );

            StartGame();
            ReturnAllTwisted();

            var decider = AssertNoDecision(SelectionType.LowestHP);
            QuickHPStorage(alexandria.CharacterCard, bitch.CharacterCard);
            PlayCard("Bullrush");
            QuickHPCheck(0, -4);

            SetHitPoints(alexandria, 10);

            QuickHPStorage(alexandria.CharacterCard, bitch.CharacterCard);
            PlayCard("Bullrush");
            QuickHPCheck(-4, 0);
            RestoreOnMakeDecisions(decider);

            SetToSameHitPoints(alexandria, bitch);

            AssertNextDecisionChoices(
                new Card[] { alexandria.CharacterCard, bitch.CharacterCard },
                new Card[] { echidna.CharacterCard }
            );

            DecisionLowestHP = bitch.CharacterCard;
            QuickHPStorage(alexandria.CharacterCard, bitch.CharacterCard);
            PlayCard("Bullrush");
            QuickHPCheck(0, -4);
        }

        [Test()]
        public void TestSearchesTheVillainDeckForAnEngulfedCard()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "Megalopolis"
            );

            StartGame();
            ReturnAllTwisted();

            var engulfedInDeck = echidna.TurnTaker.Deck.Cards.Count(c => c.IsAnEngulfedCard());
            Assert.That(engulfedInDeck, Is.GreaterThan(0));

            DecisionSelectCard = alexandria.CharacterCard;

            PlayCard("Bullrush");

            var engulfed = FindCard(c => c.IsAnEngulfedCard() && c.IsInPlay);
            Assert.That(engulfed, Is.Not.Null);
            AssertNextToCard(engulfed, alexandria.CharacterCard);
            Assert.That(
                echidna.TurnTaker.Deck.Cards.Count(c => c.IsAnEngulfedCard()),
                Is.EqualTo(engulfedInDeck - 1));
        }
    }
}
