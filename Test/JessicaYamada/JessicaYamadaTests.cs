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
    public class JessicaYamadaTests : BaseTest
    {
        protected HeroTurnTakerController jess { get { return FindHero("JessicaYamada"); } }

        [Test()]
        public void TestJessLoads()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Tachyon", "Megalopolis");

            Assert.AreEqual(4, GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(jess);
            Assert.IsInstanceOf(typeof(HeroTurnTakerController), jess);

            Assert.IsNotNull(env);

            AssertHitPoints(jess.CharacterCard, 12);
            Assert.IsTrue(jess.CharacterCard.IsHero);

            StartGame();

            AssertNotIncapacitatedOrOutOfGame(jess);

            DestroyCard(tachyon);

            AssertIncapacitated(jess);

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

            Assert.IsNotNull(jess);
            Assert.IsInstanceOf(typeof(HeroTurnTakerController), jess);

            Assert.IsNotNull(env);

            AssertNotTarget(jess.CharacterCard);
            Assert.IsTrue(jess.CharacterCard.IsHero);

            StartGame();

            AssertNotIncapacitatedOrOutOfGame(jess);

            DestroyCard(tachyon);

            AssertIncapacitated(jess);

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

            Assert.IsNotNull(jess);
            Assert.IsInstanceOf(typeof(HeroTurnTakerController), jess);

            Assert.IsNotNull(env);

            AssertHitPoints(jess.CharacterCard, 12);
            Assert.IsTrue(jess.CharacterCard.IsEnvironment);

            StartGame();

            AssertNotIncapacitatedOrOutOfGame(jess);

            // has hp, is indestructible
            DestroyCard(jess);

            AssertNotIncapacitatedOrOutOfGame(jess);

            DestroyCard(tachyon);

            AssertIncapacitated(jess);

            AssertGameOver(EndingResult.HeroesDestroyedDefeat);
        }
    }
}
