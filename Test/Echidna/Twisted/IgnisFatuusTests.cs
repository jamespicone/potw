using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Echidna
{
    [TestFixture()]
    public class IgnisFatuusTests : EchidnaBaseTest
    {
        protected HeroTurnTakerController alexandria { get { return FindHero("Alexandria"); } }
        protected HeroTurnTakerController bitch { get { return FindHero("Bitch"); } }

        protected HeroTurnTakerController legend { get { return FindHero("Legend"); } }

        [Test()]
        public void TestAddsPowerToken()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "InsulaPrimalis"
            );

            StartGame();
            DestroyNonCharacterVillainCards();
            ReturnAllTwisted();
            StackDeck("ChimaericalNightmare");

            GoToEndOfTurn(env);

            var ignis = PlayCard("IgnisFatuusTwisted");
            var powerPool = FindTokenPool("IgnisFatuusTwisted", "PowerPool");
            AssertTokenPoolCount(powerPool, 0);

            EnterNextTurnPhase();
            AssertTokenPoolCount(powerPool, 1);
        }

        [Test()]
        public void TestDoesntEnterInSetup()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "Jp.ParahumansOfTheWormverse.Legend",
                "InsulaPrimalis"
            );

            StackDeck("PropagandaTwisted", "IgnisFatuusTwisted");
            StartGame();

            AssertNotInPlay("IgnisFatuusTwisted");
            AssertNumberOfCardsInPlay(c => c.DoKeywordsContain("twisted"), 1);
        }

        [Test()]
        public void TestWinsGame()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "InsulaPrimalis"
            );

            StartGame();
            DestroyNonCharacterVillainCards();
            ReturnAllTwisted();
            StackDeck("ChimaericalNightmare");

            GoToEndOfTurn(env);

            var ignis = PlayCard("IgnisFatuusTwisted");
            var powerPool = FindTokenPool("IgnisFatuusTwisted", "PowerPool");
            powerPool.AddTokens(1);

            EnterNextTurnPhase();
            AssertGameOver(EndingResult.AlternateDefeat);
        }

        [Test()]
        public void TestLosesTokenLeavesPlay()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "InsulaPrimalis"
            );

            StartGame();
            DestroyNonCharacterVillainCards();
            ReturnAllTwisted();
            StackDeck("ChimaericalNightmare");

            GoToEndOfTurn(env);

            var ignis = PlayCard("IgnisFatuusTwisted");
            var powerPool = FindTokenPool("IgnisFatuusTwisted", "PowerPool");
            AssertTokenPoolCount(powerPool, 0);

            EnterNextTurnPhase();
            AssertTokenPoolCount(powerPool, 1);

            DestroyCard(ignis);

            ignis = PlayCard("IgnisFatuusTwisted");
            powerPool = FindTokenPool("IgnisFatuusTwisted", "PowerPool");
            AssertTokenPoolCount(powerPool, 0);
        }

        [Test()]
        public void TestDealsDamage()
        {
            SetupGameController(
                            "Jp.ParahumansOfTheWormverse.Echidna",
                            "Jp.ParahumansOfTheWormverse.Alexandria",
                            "Jp.ParahumansOfTheWormverse.Bitch",
                            "InsulaPrimalis"
                        );

            StartGame();
            DestroyNonCharacterVillainCards();
            ReturnAllTwisted();

            PlayCard("IgnisFatuusTwisted");

            StackDeck("ChimaericalNightmare");

            var effect = new CannotDealDamageStatusEffect();
            effect.SourceCriteria.IsSpecificCard = echidna.CharacterCard;
            RunCoroutine(GameController.AddStatusEffect(effect, false, echidna.CharacterCardController.GetCardSource()));

            QuickHPStorage(alexandria, bitch);
            GoToPlayCardPhase(echidna);
            QuickHPCheck(0, 0);
            GoToEndOfTurn(echidna);
            QuickHPCheck(-5, -5);
        }
    }
}
