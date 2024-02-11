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

            Assert.That(GameController.TurnTakerControllers.Count(), Is.EqualTo(4));

            Assert.That(jessica, Is.Not.Null);
            Assert.That(jessica, Is.InstanceOf<HeroTurnTakerController>());

            Assert.That(env, Is.Not.Null);

            AssertHitPoints(jessica.CharacterCard, 12);
            Assert.That(IsHeroTarget(jessica.CharacterCard, new CardSource(jessica.CharacterCardController)), Is.True);

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

            Assert.That(GameController.TurnTakerControllers.Count(), Is.EqualTo(4));

            Assert.That(jessica, Is.Not.Null);
            Assert.That(jessica, Is.InstanceOf<HeroTurnTakerController>());

            Assert.That(env, Is.Not.Null);

            AssertNotTarget(jessica.CharacterCard);
            Assert.That(IsHero(jessica.CharacterCard, new CardSource(jessica.CharacterCardController)), Is.True);

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

            Assert.That(GameController.TurnTakerControllers.Count(), Is.EqualTo(4));

            Assert.That(jessica, Is.Not.Null);
            Assert.That(jessica, Is.InstanceOf<HeroTurnTakerController>());

            Assert.That(env, Is.Not.Null);

            AssertHitPoints(jessica.CharacterCard, 12);
            Assert.That(jessica.CharacterCard.IsEnvironmentTarget, Is.True);

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

            Assert.That(jessica.CharacterCard.IsEnvironmentTarget, Is.True);
            Assert.That(IsHero(jessica.CharacterCard, new CardSource(jessica.CharacterCardController)), Is.True);

            Assert.That(jessica.CharacterCard.IsEnvironmentTarget, Is.False);
            Assert.That(IsHeroTarget(jessica.CharacterCard, new CardSource(jessica.CharacterCardController)), Is.False);
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
        #region PowerAvailable

        [Test()]
        public void TestJessCanUsePower()
        {
            SetupGameController(new[] { "BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Legacy", "Megalopolis" },
                promoIdentifiers: new Dictionary<string, string>
                {
                    { "Legacy", "AmericasGreatestLegacyCharacter" }
                }
            );

            StartGame();

            SetHitPoints(legacy, 10);

            GoToUsePowerPhase(jessica);
            AssertNumberOfUsablePowers(jessica, 1);

            QuickHPStorage(legacy);
            DecisionSelectTurnTaker = legacy.TurnTaker;
            DecisionSelectFunction = 0;
            UsePower(jessicaCharacter);
            QuickHPCheck(2);

            GoToUsePowerPhase(legacy);
            DecisionSelectCard = jessicaCharacter;
            DecisionSelectTurnTaker = legacy.TurnTaker;
            DecisionSelectFunction = 1;
            QuickHandStorage(legacy);
            UsePower(legacy);
            QuickHandCheck(1);
        }

        [Test()]
        [Ignore("Currently doesn't work because Legacy checks for incap status. Keeping around to find out if it works on later versions.")]
        public void TestNontargetJessCanUsePower()
        {
            SetupGameController(
                new[] { "BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Legacy", "Megalopolis" },
                promoIdentifiers: new Dictionary<string, string>
                {
                    { "Jp.ParahumansOfTheWormverse.JessicaYamada", "Jp.ParahumansOfTheWormverse.JessicaYamadaInstructionsNotTarget" },
                    { "Legacy", "AmericasGreatestLegacyCharacter" }
                }
            );

            StartGame();

            SetHitPoints(legacy, 10);

            GoToUsePowerPhase(jessica);
            AssertNumberOfUsablePowers(jessica, 1);

            QuickHPStorage(legacy);
            DecisionSelectTurnTaker = legacy.TurnTaker;
            DecisionSelectFunction = 0;
            UsePower(jessicaCharacter);
            QuickHPCheck(2);

            GoToUsePowerPhase(legacy);
            DecisionSelectCard = jessicaCharacter;
            DecisionSelectTurnTaker = legacy.TurnTaker;
            DecisionSelectFunction = 1;
            QuickHandStorage(legacy);
            UsePower(legacy);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestEnvironmentJessCanUsePower()
        {
            SetupGameController(
                new[] { "BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Legacy", "Megalopolis" },
                promoIdentifiers: new Dictionary<string, string>
                {
                    { "Jp.ParahumansOfTheWormverse.JessicaYamada", "Jp.ParahumansOfTheWormverse.JessicaYamadaInstructionsEnvironment" },
                    { "Legacy", "AmericasGreatestLegacyCharacter" }
                }
            );

            StartGame();

            SetHitPoints(legacy, 10);

            GoToUsePowerPhase(jessica);
            AssertNumberOfUsablePowers(jessica, 1);

            QuickHPStorage(legacy);
            DecisionSelectTurnTaker = legacy.TurnTaker;
            DecisionSelectFunction = 0;
            UsePower(jessicaCharacter);
            QuickHPCheck(2);

            GoToUsePowerPhase(legacy);
            DecisionSelectCard = jessicaCharacter;
            DecisionSelectTurnTaker = legacy.TurnTaker;
            DecisionSelectFunction = 1;
            QuickHandStorage(legacy);
            UsePower(legacy);
            QuickHandCheck(1);
        }

        #endregion
    }
}
