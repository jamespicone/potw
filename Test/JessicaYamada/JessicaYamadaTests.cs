using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;
using Jp.ParahumansOfTheWormverse.Bitch;

namespace Jp.ParahumansOfTheWormverse.UnitTest.JessicaYamada
{
    [TestFixture()]
    public class JessicaYamadaTests : ParahumanTest
    {
        [Test()]
        public void TestJessLoads()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Tachyon", "Megalopolis");

            Assert.AreEqual(4, GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(jessica);
            Assert.IsInstanceOf(typeof(HeroTurnTakerController), jessica);

            Assert.IsNotNull(env);

            AssertHitPoints(jessica.CharacterCard, 12);
            Assert.IsTrue(jessica.CharacterCard.IsHero);

            StartGame();

            AssertNotIncapacitatedOrOutOfGame(jessica);

            DestroyCard(tachyon);

            AssertIncapacitated(jessica);

            AssertGameOver(EndingResult.HeroesDestroyedDefeat);
        }

        [Test()]
        public void TestJessLoadsNotTarget()
        {
            SetupGameController(
                new[] { "BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Tachyon", "Megalopolis" },
                promoIdentifiers: new Dictionary<string, string>
                {
                    { "Jp.ParahumansOfTheWormverse.JessicaYamada", "Jp.ParahumansOfTheWormverse.JessicaYamadaInstructionsNotTarget" }
                }
                );

            Assert.AreEqual(4, GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(jessica);
            Assert.IsInstanceOf(typeof(HeroTurnTakerController), jessica);

            Assert.IsNotNull(env);

            AssertNotTarget(jessica.CharacterCard);
            Assert.IsTrue(jessica.CharacterCard.IsHero);

            StartGame();

            AssertNotIncapacitatedOrOutOfGame(jessica);

            DestroyCard(tachyon);

            AssertIncapacitated(jessica);

            AssertGameOver(EndingResult.HeroesDestroyedDefeat);
        }

        [Test()]
        public void TestJessLoadsEnvironment()
        {
            SetupGameController(
                new[] { "BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Tachyon", "Megalopolis" },
                promoIdentifiers: new Dictionary<string, string>
                {
                    { "Jp.ParahumansOfTheWormverse.JessicaYamada", "Jp.ParahumansOfTheWormverse.JessicaYamadaInstructionsEnvironment" }
                }
                );

            Assert.AreEqual(4, GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(jessica);
            Assert.IsInstanceOf(typeof(HeroTurnTakerController), jessica);

            Assert.IsNotNull(env);

            AssertHitPoints(jessica.CharacterCard, 12);
            Assert.IsTrue(jessica.CharacterCard.IsEnvironment);

            StartGame();

            AssertNotIncapacitatedOrOutOfGame(jessica);

            // has hp, is indestructible
            DestroyCard(jessica);

            AssertNotIncapacitatedOrOutOfGame(jessica);

            DestroyCard(tachyon);

            AssertIncapacitated(jessica);

            AssertGameOver(EndingResult.HeroesDestroyedDefeat);
        }
    }
}
