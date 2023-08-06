using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Jp.ParahumansOfTheWormverse.Labyrinth;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Labyrinth
{
    [TestFixture()]
    public class DissassoctionTests : ParahumanTest
    {
        [Test()]
        public void TestPower()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Legacy", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            var disassociation = PlayCard("Disassociation");

            var garden = PutInHand("BeautifulGarden");
            var field = GetCard("ObsidianField");
            StackDeck("ObsidianField");

            DecisionSelectCardToPlay = garden;

            UsePower(disassociation);

            AssertIsInPlay(garden);
            AssertUnderCard(garden, field);
        }

        [Test()]
        public void TestPowerEmptyDeck()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Legacy", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            var disassociation = PlayCard("Disassociation");

            var garden = PutInHand("BeautifulGarden");

            MoveAllCards(env, env.TurnTaker.Deck, env.TurnTaker.Trash);
            DecisionSelectCardToPlay = garden;

            UsePower(disassociation);

            AssertInTrash(garden);
            AssertNumberOfCardsInTrash(env, 15);
        }

        [Test()]
        public void TestPsychicImmune()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Legacy", "InsulaPrimalis");

            PlayCard("Disassociation");

            QuickHPStorage(labyrinth);
            DealDamage(labyrinth, labyrinth, 20, DamageType.Psychic);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestEndTurnDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Legacy", "InsulaPrimalis");

            RemoveVillainCards();
            RemoveVillainTriggers();

            StartGame();

            GoToPlayCardPhase(labyrinth);
            
            PlayCard("Disassociation");

            var vantage = PutInHand("Vantage");
            DecisionSelectCard = vantage;

            QuickHPStorage(labyrinth);
            GoToEndOfTurn(labyrinth);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestEndTurnPlay()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Legacy", "InsulaPrimalis");

            RemoveVillainCards();
            RemoveVillainTriggers();

            StartGame();

            GoToPlayCardPhase(labyrinth);

            PlayCard("Disassociation");

            var vantage = PutInHand("Vantage");
            DecisionSelectCard = vantage;

            GoToEndOfTurn(labyrinth);

            AssertIsInPlay(vantage);
        }

        [Test()]
        public void TestEndTurnPlayIsOptional()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Legacy", "InsulaPrimalis");

            RemoveVillainCards();
            RemoveVillainTriggers();

            StartGame();

            GoToPlayCardPhase(labyrinth);

            PlayCard("Disassociation");

            MoveAllCardsFromHandToDeck(labyrinth);
            var vantage = PutInHand("Vantage");
            MoveAllCards(labyrinth, labyrinth.TurnTaker.Deck, labyrinth.TurnTaker.OutOfGame);

            DecisionDoNotSelectCard = SelectionType.PlayCard;
            GoToEndOfTurn(labyrinth);

            AssertInHand(vantage);
        }
    }
}
