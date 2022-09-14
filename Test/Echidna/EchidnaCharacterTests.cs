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
    public class EchidnaCharacterTests : EchidnaBaseTest
    {
        protected HeroTurnTakerController alexandria { get { return FindHero("Alexandria"); } }
        protected HeroTurnTakerController bitch { get { return FindHero("Bitch"); } }
        protected HeroTurnTakerController armsmaster { get { return FindHero("Armsmaster"); } }
        protected HeroTurnTakerController legend { get { return FindHero("Legend"); } }

        [Test()]
        public void TestRegainHP()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "InsulaPrimalis"
            );


            SetHitPoints(echidna, 30);
            QuickHPStorage(echidna);
            StartGame();
            QuickHPCheck(2);
        }

        [Test()]
        public void TestEOTDamage()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "Jp.ParahumansOfTheWormverse.Armsmaster",
                "Jp.ParahumansOfTheWormverse.Legend",
                "InsulaPrimalis"
            );

            StartGame();
            RemoveAllTwisted();

            StackDeck(echidna, "ChimaericalNightmare");

            SetHitPoints(alexandria, 28);
            SetHitPoints(bitch, 27);
            SetHitPoints(armsmaster, 26);
            SetHitPoints(legend, 25);

            QuickHPStorage(alexandria, bitch, armsmaster, legend);

            GoToEndOfTurn(echidna);

            QuickHPCheck(0, 0, -3, -3);
        }

        [Test()]
        public void TestDidDamageWontFlip()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "Jp.ParahumansOfTheWormverse.Armsmaster",
                "Jp.ParahumansOfTheWormverse.Legend",
                "InsulaPrimalis"
            );

            StartGame();
            RemoveAllTwisted();

            StackDeck(echidna, "ChimaericalNightmare");
            StackDeck(env, "ObsidianField");

            GoToStartOfTurn(env);

            AssertNotFlipped(echidna);

            GoToEndOfTurn(env);

            AssertNotFlipped(echidna);

            GoToStartOfTurn(echidna);

            AssertNotFlipped(echidna);
        }

        [Test()]
        public void TestDidntDamageWillFlip()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "Jp.ParahumansOfTheWormverse.Armsmaster",
                "Jp.ParahumansOfTheWormverse.Legend",
                "InsulaPrimalis"
            );

            StartGame();
            RemoveAllTwisted();

            StackDeck(echidna, "ChimaericalNightmare");
            StackDeck(env, "ObsidianField");

            var effect = new ImmuneToDamageStatusEffect();
            effect.TargetCriteria.IsHero = true;

            RunCoroutine(GameController.AddStatusEffect(effect, false, alexandria.CharacterCardController.GetCardSource()));

            GoToStartOfTurn(env);

            AssertNotFlipped(echidna);

            GoToEndOfTurn(env);

            AssertFlipped(echidna);
        }

        [Test()]
        public void TestFlippedFlipsBack()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "Jp.ParahumansOfTheWormverse.Armsmaster",
                "Jp.ParahumansOfTheWormverse.Legend",
                "InsulaPrimalis"
            );

            FlipCard(echidna);
            StackDeck(echidna, "ChimaericalNightmare");

            DecisionNextToCard = legend.CharacterCard;

            StartGame(false);
            RemoveAllTwisted();

            AssertNotFlipped(echidna);

            AssertNumberOfCardsNextToCard(legend.CharacterCard, 1);
        }
    }
}
