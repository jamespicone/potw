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
    public class PageThroughTests : BaseTest
    {
        protected HeroTurnTakerController labyrinth { get { return FindHero("Labyrinth"); } }
        protected HeroTurnTakerController tattletale { get { return FindHero("Tattletale"); } }

        [Test()]
        public void TestStashesCards()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Jp.ParahumansOfTheWormverse.Tattletale", "Legacy", "InsulaPrimalis");

            RemoveVillainTriggers();
            MoveAllCards(FindEnvironment(), FindEnvironment().TurnTaker.Deck, FindEnvironment().TurnTaker.OutOfGame);
            MoveAllCards(baron, baron.TurnTaker.Deck, baron.TurnTaker.OutOfGame);
            StartGame();


            StackDeck("ObsidianField", "VelociraptorPack");
            GoToStartOfTurn(labyrinth);
            var raptors = env.TurnTaker.Deck.TopCard;

            PlayCard("PageThrough");
            GoToEndOfTurn();
            AssertUnderCard(labyrinth.CharacterCard, raptors);
        }

        [Test()]
        public void TestPowerDraws()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Jp.ParahumansOfTheWormverse.Tattletale", "Legacy", "InsulaPrimalis");

            RemoveVillainTriggers();
            MoveAllCards(FindEnvironment(), FindEnvironment().TurnTaker.Deck, FindEnvironment().TurnTaker.OutOfGame);
            MoveAllCards(baron, baron.TurnTaker.Deck, baron.TurnTaker.OutOfGame);
            StartGame();

            StackDeck("ObsidianField", "VelociraptorPack");
            var page = PlayCard("PageThrough");

            QuickHandStorage(labyrinth);
            UsePower(page);
            QuickHandCheck(2);
        }

        [Test()]
        public void TestPowerPutsCardsInPlay()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Jp.ParahumansOfTheWormverse.Tattletale", "Legacy", "InsulaPrimalis");

            RemoveVillainTriggers();
            MoveAllCards(FindEnvironment(), FindEnvironment().TurnTaker.Deck, FindEnvironment().TurnTaker.OutOfGame);
            MoveAllCards(baron, baron.TurnTaker.Deck, baron.TurnTaker.OutOfGame);
            StartGame();

            var topcards = StackDeck("ObsidianField", "VelociraptorPack");
            var page = PlayCard("PageThrough");

            DecisionDoNotSelectCard = SelectionType.DiscardCard;
            UsePower(page);

            AssertIsInPlay(topcards);
        }

        [Test()]
        public void TestPowerDiscardsOne()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Jp.ParahumansOfTheWormverse.Tattletale", "Legacy", "InsulaPrimalis");

            RemoveVillainTriggers();
            MoveAllCards(FindEnvironment(), FindEnvironment().TurnTaker.Deck, FindEnvironment().TurnTaker.OutOfGame);
            MoveAllCards(baron, baron.TurnTaker.Deck, baron.TurnTaker.OutOfGame);
            StartGame();

            var topcards = StackDeck("ObsidianField", "VelociraptorPack");
            var page = PlayCard("PageThrough");

            DecisionSelectCard = topcards.First();
            UsePower(page);

            AssertIsInPlay(topcards.Skip(1).First());
            AssertInTrash(topcards.First());
        }
    }
}
