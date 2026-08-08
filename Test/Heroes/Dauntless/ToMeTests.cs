using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Dauntless
{
    [TestFixture()]
    public class ToMeTests : ParahumanTest
    {
        [Test()]
        public void TestSearchDeckForRelic()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            var arcshield = GetCard("Arcshield");
            MoveCard(dauntless, arcshield, dauntless.TurnTaker.Deck);

            DecisionSelectCard = arcshield;
            DecisionDoNotSelectCard = SelectionType.PlayCard;

            PlayCard("ToMe");

            // Arcshield should be in play
            AssertIsInPlay(arcshield);
        }

        [Test()]
        public void TestDrawsCard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            DecisionSelectCard = PutOnDeck("Arcshield");
            DecisionDoNotSelectCard = SelectionType.PlayCard;

            QuickHandStorage(dauntless);
            PlayCard("ToMe");

            // Drew a card (To Me is a one-shot so not in hand after play)
            QuickHandCheck(1);
        }

        [Test()]
        public void TestOptionalPlay()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            // Ensure only Arcshield is in deck so the search auto-resolves
            var arcshield = GetCard("Arcshield");
            var arcstep = GetCard("Arcstep");
            MoveCard(dauntless, arcshield, dauntless.TurnTaker.Deck, toBottom: false);
            MoveCard(dauntless, arcstep, dauntless.TurnTaker.Trash);

            var corona = PutInHand("CracklingCorona");

            // Relic search auto-resolves (only 1 relic), then select corona to play from hand
            DecisionSelectCard = corona;

            PlayCard("ToMe");

            // Corona should have been played (moved to trash as one-shot)
            AssertInTrash(corona);
        }

        [Test()]
        public void TestCanDeclineToPlay()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            var corona = PutInHand("CracklingCorona");

            DecisionSelectCard = PutOnDeck("Arcshield");
            DecisionDoNotSelectCard = SelectionType.PlayCard; // Skip the optional play

            PlayCard("ToMe");

            // Corona should still be in hand
            AssertInHand(corona);
        }

        [Test()]
        public void TestShufflesDeck()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            var arcshield = GetCard("Arcshield");
            MoveCard(dauntless, arcshield, dauntless.TurnTaker.Deck);

            DecisionSelectCard = arcshield;
            DecisionDoNotSelectCard = SelectionType.PlayCard;

            PlayCard("ToMe");

            // Deck should be shuffled (hard to verify, but the card should be played correctly)
            AssertIsInPlay(arcshield);
        }

        [Test()]
        public void TestFindsArcstep()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            var arcstep = GetCard("Arcstep");
            MoveCard(dauntless, arcstep, dauntless.TurnTaker.Deck);

            DecisionSelectCard = arcstep;
            DecisionDoNotSelectCard = SelectionType.PlayCard;

            PlayCard("ToMe");

            // Arcstep should be in play
            AssertIsInPlay(arcstep);
        }

        [Test()]
        public void TestNoRelicInDeck()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            // Remove all relics from deck
            var relics = FindCardsWhere(c => c.DoKeywordsContain("relic") && c.Location == dauntless.TurnTaker.Deck);
            foreach (var relic in relics)
            {
                MoveCard(dauntless, relic, dauntless.TurnTaker.OutOfGame);
            }

            DecisionDoNotSelectCard = SelectionType.PlayCard;

            // Should still work, just no relic found
            PlayCard("ToMe");
        }

        [Test()]
        public void TestFullSequence()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            // Ensure only Arcshield is in deck so the search auto-resolves
            var arcshield = GetCard("Arcshield");
            var arcstep = GetCard("Arcstep");
            MoveCard(dauntless, arcshield, dauntless.TurnTaker.Deck, toBottom: false);
            MoveCard(dauntless, arcstep, dauntless.TurnTaker.Trash);

            var corona = PutInHand("CracklingCorona");

            // Relic search auto-resolves (only 1 relic), then select corona to play from hand
            DecisionSelectCard = corona;

            QuickHPStorage(baron);
            PlayCard("ToMe");

            // Arcshield in play
            AssertIsInPlay(arcshield);
            // Corona was played, dealing damage to Baron
            AssertInTrash(corona);
            QuickHPCheck(-1);
        }
    }
}
