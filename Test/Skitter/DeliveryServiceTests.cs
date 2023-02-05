using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Jp.ParahumansOfTheWormverse.Skitter;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Skitter
{
    [TestFixture()]
    public class DeliveryServiceTests : BaseTest
    {
        protected HeroTurnTakerController skitter { get { return FindHero("Skitter"); } }
        protected HeroTurnTakerController tattletale { get { return FindHero("Tattletale"); } }

        [Test()]
        public void ZeroTokensOneDraw()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            PlayCard("DeliveryService");

            GoToDrawCardPhase(skitter);

            QuickHandStorage(skitter, tattletale);
            DecisionSelectTurnTaker = skitter.TurnTaker;
            GoToEndOfTurn(skitter);
            QuickHandCheck(1, 0);
        }

        [Test()]
        public void OneTokenTwoDraws()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            var delivery = PlayCard("DeliveryService");

            var bugpool = delivery.FindBugPool();
            bugpool.AddTokens(1);
            GoToDrawCardPhase(skitter);

            QuickHandStorage(skitter, tattletale);
            DecisionSelectTurnTaker = tattletale.TurnTaker;
            GoToEndOfTurn(skitter);
            QuickHandCheck(0, 2);
        }

        [Test()]
        public void TwoTokensThreeDraws()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            var delivery = PlayCard("DeliveryService");

            var bugpool = delivery.FindBugPool();
            bugpool.AddTokens(2);
            GoToDrawCardPhase(skitter);

            QuickHandStorage(skitter, tattletale);
            DecisionSelectTurnTaker = tattletale.TurnTaker;
            GoToEndOfTurn(skitter);
            QuickHandCheck(0, 3);
        }

        [Test()]
        public void ThreeTokensFourDraws()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            var delivery = PlayCard("DeliveryService");

            var bugpool = delivery.FindBugPool();
            bugpool.AddTokens(3);
            GoToDrawCardPhase(skitter);

            QuickHandStorage(skitter, tattletale);
            DecisionSelectTurnTaker = tattletale.TurnTaker;
            GoToEndOfTurn(skitter);
            QuickHandCheck(0, 4);
        }

        [Test()]
        public void FourTokensFourDraws()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            var delivery = PlayCard("DeliveryService");

            var bugpool = delivery.FindBugPool();
            bugpool.AddTokens(4);
            GoToDrawCardPhase(skitter);

            QuickHandStorage(skitter, tattletale);
            DecisionSelectTurnTaker = tattletale.TurnTaker;
            GoToEndOfTurn(skitter);
            QuickHandCheck(0, 4);
        }
    }
}
