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
    public class InspirationTests : BaseTest
    {
        protected HeroTurnTakerController labyrinth { get { return FindHero("Labyrinth"); } }
        protected HeroTurnTakerController tattletale { get { return FindHero("Tattletale"); } }

        [Test()]
        public void TestDrawsWhenEnvEntersPlay()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Jp.ParahumansOfTheWormverse.Tattletale", "Legacy", "InsulaPrimalis");

            RemoveVillainTriggers();
            MoveAllCards(FindEnvironment(), FindEnvironment().TurnTaker.Deck, FindEnvironment().TurnTaker.OutOfGame);
            MoveAllCards(baron, baron.TurnTaker.Deck, baron.TurnTaker.OutOfGame);
            StartGame();

            PlayCard("Inspiration");
            QuickHandStorage(labyrinth);
            PlayCard("ObsidianField");
            QuickHandCheck(1);
        }

        [Test()]
        public void TestDestroyEnvForPlay()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Jp.ParahumansOfTheWormverse.Tattletale", "Legacy", "InsulaPrimalis");

            RemoveVillainTriggers();
            MoveAllCards(FindEnvironment(), FindEnvironment().TurnTaker.Deck, FindEnvironment().TurnTaker.OutOfGame);
            MoveAllCards(baron, baron.TurnTaker.Deck, baron.TurnTaker.OutOfGame);
            StartGame();

            var field = PlayCard("ObsidianField");
            var inspiration = PlayCard("Inspiration");
            var vantage = PutInHand("Vantage");

            DecisionSelectCardToPlay = vantage;
            UsePower(inspiration);

            AssertInTrash(field);
            AssertIsInPlay(vantage);
        }
    }
}
