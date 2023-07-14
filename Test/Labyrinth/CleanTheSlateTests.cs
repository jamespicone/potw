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
    public class CleanTheSlateTests : BaseTest
    {
        protected HeroTurnTakerController labyrinth { get { return FindHero("Labyrinth"); } }
        protected HeroTurnTakerController tattletale { get { return FindHero("Tattletale"); } }

        [Test()]
        public void TestDestroyZeroCards()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "InsulaPrimalis");

            StartGame();
            RemoveVillainCards();
            MoveAllCardsFromHandToDeck(labyrinth);

            var page1 = PutInHand(labyrinth, "PageThrough", 0);
            var page2 = PutInHand(labyrinth, "PageThrough", 0);
            var page3 = PutInHand(labyrinth, "PageThrough", 0);

            var topcards = StackDeck("CallInTheCrew", "Exploration", "Inspiration");
            var top = topcards.ElementAt(2);
            var middle = topcards.ElementAt(1);
            var bottom = topcards.ElementAt(0);

            var field = PlayCard("ObsidianField");
            var raptors = PlayCard("VelociraptorPack");
            var rex = PlayCard("EnragedTRex");

            DecisionDoNotSelectCard = SelectionType.DestroyCard;
            PlayCard("CleanTheSlate");

            AssertIsInPlay(field);
            AssertIsInPlay(raptors);
            AssertIsInPlay(rex);

            AssertInHand(page1);
            AssertInHand(page2);
            AssertInHand(page3);

            AssertInDeck(top);
            AssertInDeck(middle);
            AssertInDeck(bottom);
        }

        [Test()]
        public void TestDestroyOneCards()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "InsulaPrimalis");

            StartGame();
            RemoveVillainCards();
            MoveAllCardsFromHandToDeck(labyrinth);

            var page1 = PutInHand(labyrinth, "PageThrough", 0);
            var page2 = PutInHand(labyrinth, "PageThrough", 0);
            var page3 = PutInHand(labyrinth, "PageThrough", 0);

            var topcards = StackDeck("CallInTheCrew", "Exploration", "Inspiration");
            var top = topcards.ElementAt(2);
            var middle = topcards.ElementAt(1);
            var bottom = topcards.ElementAt(0);

            var field = PlayCard("ObsidianField");
            var raptors = PlayCard("VelociraptorPack");
            var rex = PlayCard("EnragedTRex");

            DecisionSelectCards = new Card[] { field, null, page1 };
            PlayCard("CleanTheSlate");

            AssertInTrash(field);
            AssertIsInPlay(raptors);
            AssertIsInPlay(rex);

            AssertIsInPlay(page1);
            AssertInHand(page2);
            AssertInHand(page3);

            AssertInHand(top);
            AssertInDeck(middle);
            AssertInDeck(bottom);
        }

        [Test()]
        public void TestDestroyTwoCards()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "InsulaPrimalis");

            StartGame();
            RemoveVillainCards();
            MoveAllCardsFromHandToDeck(labyrinth);

            var page1 = PutInHand(labyrinth, "PageThrough", 0);
            var page2 = PutInHand(labyrinth, "PageThrough", 0);
            var page3 = PutInHand(labyrinth, "PageThrough", 0);

            var topcards = StackDeck("CallInTheCrew", "Exploration", "Inspiration");
            var top = topcards.ElementAt(2);
            var middle = topcards.ElementAt(1);
            var bottom = topcards.ElementAt(0);

            var field = PlayCard("ObsidianField");
            var raptors = PlayCard("VelociraptorPack");
            var rex = PlayCard("EnragedTRex");

            DecisionSelectCards = new Card[] { field, raptors, null, page1, page2 };
            PlayCard("CleanTheSlate");

            AssertInTrash(field);
            AssertInTrash(raptors);
            AssertIsInPlay(rex);

            AssertIsInPlay(page1);
            AssertIsInPlay(page2);
            AssertInHand(page3);

            AssertInHand(top);
            AssertInHand(middle);
            AssertInDeck(bottom);
        }

        [Test()]
        public void TestDestroyThreeCards()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "InsulaPrimalis");

            StartGame();
            RemoveVillainCards();
            MoveAllCardsFromHandToDeck(labyrinth);

            var page1 = PutInHand(labyrinth, "PageThrough", 0);
            var page2 = PutInHand(labyrinth, "PageThrough", 0);
            var page3 = PutInHand(labyrinth, "PageThrough", 0);

            var topcards = StackDeck("CallInTheCrew", "Exploration", "Inspiration");
            var top = topcards.ElementAt(2);
            var middle = topcards.ElementAt(1);
            var bottom = topcards.ElementAt(0);

            var field = PlayCard("ObsidianField");
            var raptors = PlayCard("VelociraptorPack");
            var rex = PlayCard("EnragedTRex");

            DecisionSelectCards = new Card[] { field, raptors, rex, page1, page2, page3 };
            PlayCard("CleanTheSlate");

            AssertInTrash(field);
            AssertInTrash(raptors);
            AssertInTrash(rex);

            AssertIsInPlay(page1);
            AssertIsInPlay(page2);
            AssertIsInPlay(page3);

            AssertInHand(top);
            AssertInHand(middle);
            AssertInHand(bottom);
        }
    }
}
