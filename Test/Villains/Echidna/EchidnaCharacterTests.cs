﻿using System;
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
    public class EchidnaCharacterTests : ParahumanTest
    {
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

            RemoveAllTwisted();
            StartGame();            

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

            RemoveAllTwisted();
            StartGame();            

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


            RemoveAllTwisted();
            StartGame();            

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
        public void TestDidDamageWontFlipEnvDamage()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "Jp.ParahumansOfTheWormverse.Armsmaster",
                "Jp.ParahumansOfTheWormverse.Legend",
                "InsulaPrimalis"
            );

            RemoveAllTwisted();
            StartGame();

            StackDeck(echidna, "ChimaericalNightmare");
            StackDeck(env, "ObsidianField");

            GoToStartOfTurn(env);

            AssertNotFlipped(echidna);

            DealDamage(FindEnvironment().TurnTaker, alexandria.CharacterCard, 50, DamageType.Infernal);

            GoToEndOfTurn(env);

            AssertNotFlipped(echidna);

            GoToStartOfTurn(echidna);

            AssertNotFlipped(echidna);
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

            RemoveAllTwisted();
            StartGame(false);            

            AssertNotFlipped(echidna);

            AssertNumberOfCardsNextToCard(legend.CharacterCard, 1);
        }
    }
}
