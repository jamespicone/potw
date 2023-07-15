using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.JessicaYamada
{
    [TestFixture()]
    public class JessicaYamadaTests : ParahumanTest
    {
        #region Loading

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
            AssertFlipped(jessicaCharacter);
            AssertFlipped(jessicaInstructions);

            AssertGameOver(EndingResult.HeroesDestroyedDefeat);
        }

        [Test()]
        public void TestNonTargetJessLoads()
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
            AssertFlipped(jessicaCharacter);
            AssertFlipped(jessicaInstructions);

            AssertGameOver(EndingResult.HeroesDestroyedDefeat);
        }

        [Test()]
        public void TestEnvironmentJessLoads()
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

            StartGame();

            AssertNotIncapacitatedOrOutOfGame(jessica);

            // has hp, is indestructible
            DestroyCard(jessica);

            AssertNotIncapacitatedOrOutOfGame(jessica);

            DestroyCard(tachyon);

            AssertIncapacitated(jessica);
            AssertFlipped(jessicaCharacter);
            AssertFlipped(jessicaInstructions);

            AssertGameOver(EndingResult.HeroesDestroyedDefeat);
        }

        #endregion
        #region Damage

        [Test()]
        public void TestJessIncappedByDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Tachyon", "Megalopolis");

            StartGame();

            AssertNotIncapacitatedOrOutOfGame(jessica);

            DealDamage(tachyon, jessica, 30, DamageType.Infernal);

            AssertIncapacitated(jessica);

            AssertFlipped(jessicaCharacter);
            AssertFlipped(jessicaInstructions);
        }

        

        [Test()]
        public void TestEnvironmentJessIncappedByDamage()
        {
            SetupGameController(
                new[] { "BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Tachyon", "Megalopolis" },
                promoIdentifiers: new Dictionary<string, string>
                {
                    { "Jp.ParahumansOfTheWormverse.JessicaYamada", "Jp.ParahumansOfTheWormverse.JessicaYamadaInstructionsEnvironment" }
                }
                );

            StartGame();

            AssertNotIncapacitatedOrOutOfGame(jessica);

            DealDamage(baron, jessica, 30, DamageType.Infernal);

            AssertIncapacitated(jessica);

            AssertFlipped(jessicaCharacter);
            AssertFlipped(jessicaInstructions);
        }

        #endregion
        #region Kind

        [Test()]
        public void TestEnvironmentJessKind()
        {
            SetupGameController(
                new[] { "BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Tachyon", "Megalopolis" },
                promoIdentifiers: new Dictionary<string, string>
                {
                    { "Jp.ParahumansOfTheWormverse.JessicaYamada", "Jp.ParahumansOfTheWormverse.JessicaYamadaInstructionsEnvironment" }
                }
                );

            StartGame();

            Assert.IsTrue(jessica.CharacterCard.IsEnvironmentTarget);
            Assert.IsTrue(IsHero(jessica.CharacterCard, new CardSource(jessica.CharacterCardController)));

            Assert.IsFalse(jessica.CharacterCard.IsEnvironment);
            Assert.IsFalse(IsHeroTarget(jessica.CharacterCard, new CardSource(jessica.CharacterCardController)));
        }

        #endregion
        #region CantDealDamage

        [Test()]
        public void TestJessCantDealDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Tachyon", "Megalopolis");

            StartGame();

            QuickHPStorage(tachyon);
            DealDamage(jessica, tachyon, 10, DamageType.Infernal);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestNontargetJessCantDealDamage()
        {
            SetupGameController(
                new[] { "BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Tachyon", "Megalopolis" },
                promoIdentifiers: new Dictionary<string, string>
                {
                    { "Jp.ParahumansOfTheWormverse.JessicaYamada", "Jp.ParahumansOfTheWormverse.JessicaYamadaInstructionsNotTarget" }
                }
                );

            StartGame();

            QuickHPStorage(tachyon);
            DealDamage(jessica, tachyon, 10, DamageType.Infernal);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestEnvironmentJessCantDealDamage()
        {
            SetupGameController(
                new[] { "BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Tachyon", "Megalopolis" },
                promoIdentifiers: new Dictionary<string, string>
                {
                    { "Jp.ParahumansOfTheWormverse.JessicaYamada", "Jp.ParahumansOfTheWormverse.JessicaYamadaInstructionsEnvironment" }
                }
                );

            StartGame();

            QuickHPStorage(tachyon);
            DealDamage(jessica, tachyon, 10, DamageType.Infernal);
            QuickHPCheck(0);
        }

        #endregion
        #region DestroyedDirectly

        [Test()]
        public void TestJessDirectlyIncapped()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Tachyon", "Megalopolis");

            StartGame();

            AssertNotIncapacitatedOrOutOfGame(jessica);

            FlipCard(jessicaCharacter);

            AssertIncapacitated(jessica);
            AssertFlipped(jessicaCharacter);
            AssertFlipped(jessicaInstructions);
        }

        [Test()]
        public void TestNontargetJessDirectlyIncapped()
        {
            SetupGameController(
                new[] { "BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Tachyon", "Megalopolis" },
                promoIdentifiers: new Dictionary<string, string>
                {
                    { "Jp.ParahumansOfTheWormverse.JessicaYamada", "Jp.ParahumansOfTheWormverse.JessicaYamadaInstructionsNotTarget" }
                }
                );

            StartGame();

            AssertNotIncapacitatedOrOutOfGame(jessica);

            FlipCard(jessicaCharacter);

            AssertIncapacitated(jessica);
            AssertFlipped(jessicaCharacter);
            AssertFlipped(jessicaInstructions);
        }

        [Test()]
        public void TestEnvironmentJessDirectlyIncapped()
        {
            SetupGameController(
                new[] { "BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Tachyon", "Megalopolis" },
                promoIdentifiers: new Dictionary<string, string>
                {
                    { "Jp.ParahumansOfTheWormverse.JessicaYamada", "Jp.ParahumansOfTheWormverse.JessicaYamadaInstructionsEnvironment" }
                }
                );

            StartGame();

            AssertNotIncapacitatedOrOutOfGame(jessica);

            FlipCard(jessicaCharacter);

            AssertIncapacitated(jessica);
            AssertFlipped(jessicaCharacter);
            AssertFlipped(jessicaInstructions);
        }

        #endregion
        #region Unincap

        [Test()]
        public void TestJessUnincapped()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Tachyon", "TheTempleOfZhuLong");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            AssertNotIncapacitatedOrOutOfGame(jessica);

            FlipCard(jessicaCharacter);
            AssertIncapacitated(jessica);
            AssertFlipped(jessicaCharacter);
            AssertFlipped(jessicaInstructions);

            DrawCard(tachyon, 40);

            DecisionSelectCards = tachyon.HeroTurnTaker.Hand.Cards.Take(10).Append(null);
            DecisionSelectTurnTakers = new TurnTaker[]{ tachyon.TurnTaker, null, jessica.TurnTaker };

            GoToPlayCardPhaseAndPlayCard(env, "RitesOfRevival");
            EnterNextTurnPhase();

            AssertNotIncapacitatedOrOutOfGame(jessica);
            AssertNotFlipped(jessicaCharacter);
            AssertNotFlipped(jessicaInstructions);
        }

        [Test()]
        public void TestNontargetJessUnincapped()
        {
            SetupGameController(
                new[] { "BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Tachyon", "TheTempleOfZhuLong" },
                promoIdentifiers: new Dictionary<string, string>
                {
                    { "Jp.ParahumansOfTheWormverse.JessicaYamada", "Jp.ParahumansOfTheWormverse.JessicaYamadaInstructionsNotTarget" }
                }
                );

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            AssertNotIncapacitatedOrOutOfGame(jessica);

            FlipCard(jessicaCharacter);
            AssertIncapacitated(jessica);
            AssertFlipped(jessicaCharacter);
            AssertFlipped(jessicaInstructions);

            DrawCard(tachyon, 40);

            DecisionSelectCards = tachyon.HeroTurnTaker.Hand.Cards.Take(10).Append(null);
            DecisionSelectTurnTakers = new TurnTaker[] { tachyon.TurnTaker, null, jessica.TurnTaker };

            GoToPlayCardPhaseAndPlayCard(env, "RitesOfRevival");
            EnterNextTurnPhase();

            AssertNotIncapacitatedOrOutOfGame(jessica);
            AssertNotFlipped(jessicaCharacter);
            AssertNotFlipped(jessicaInstructions);
        }

        [Test()]
        public void TestEnvironmentJessUnincapped()
        {
            SetupGameController(
                new[] { "BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Tachyon", "TheTempleOfZhuLong" },
                promoIdentifiers: new Dictionary<string, string>
                {
                    { "Jp.ParahumansOfTheWormverse.JessicaYamada", "Jp.ParahumansOfTheWormverse.JessicaYamadaInstructionsEnvironment" }
                }
                );

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            AssertNotIncapacitatedOrOutOfGame(jessica);

            FlipCard(jessicaCharacter);
            AssertIncapacitated(jessica);
            AssertFlipped(jessicaCharacter);
            AssertFlipped(jessicaInstructions);

            DrawCard(tachyon, 40);

            DecisionSelectCards = tachyon.HeroTurnTaker.Hand.Cards.Take(10).Append(null);
            DecisionSelectTurnTakers = new TurnTaker[] { tachyon.TurnTaker, null, jessica.TurnTaker };

            GoToPlayCardPhaseAndPlayCard(env, "RitesOfRevival");
            EnterNextTurnPhase();

            AssertNotIncapacitatedOrOutOfGame(jessica);
            AssertNotFlipped(jessicaCharacter);
            AssertNotFlipped(jessicaInstructions);
        }

        #endregion
    }
}
