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
    public class CallInTheCrewTests : BaseTest
    {
        protected HeroTurnTakerController labyrinth { get { return FindHero("Labyrinth"); } }
        protected HeroTurnTakerController tattletale { get { return FindHero("Tattletale"); } }

        /*
         * "When you use a power on this card put a token on it. Powers with tokens on them cannot be used.",
         * "Put a non-indestructible noncharacter target on top of its deck.
         * {LabyrinthCharacter} deals 3 sonic damage to all non-hero targets.
         * Select a damage type. Until the start of your next turn whenever damage of that type would be dealt reduce it by 1.
         * {LabyrinthCharacter} deals a target 4 fire damage.
         * All players draw a card.
         */
        [Test()]
        public void TestFirstPower()
        {
            // Put a non-indestructible noncharacter target on top of its deck
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Jp.ParahumansOfTheWormverse.Tattletale", "InsulaPrimalis");

            StartGame();

            var crew = PlayCard("CallInTheCrew");

            var platform = GetMobileDefensePlatform().Card;
            DecisionSelectCard = platform;

            UsePower(crew, 0);
            AssertOnTopOfDeck(platform);
            AssertInDeck(baron, platform);

            RemoveVillainTriggers();

            GoToNextTurn();

            var powers = GetUsablePowersThisTurn(labyrinth);
            Assert.AreEqual(powers.Count(p => p.Index == 0 && p.CardSource.Card == crew), 0);
        }

        [Test()]
        public void TestSecondPower()
        {
            // {LabyrinthCharacter} deals 3 sonic damage to all non-hero targets
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Jp.ParahumansOfTheWormverse.Tattletale", "InsulaPrimalis");

            StartGame();

            var crew = PlayCard("CallInTheCrew");

            var platform = GetMobileDefensePlatform().Card;
            var raptors = PlayCard("VelociraptorPack");

            QuickHPStorage(baron.CharacterCard, platform, raptors);
            UsePower(crew, 1);
            QuickHPCheck(0, -3, -3);            

            RemoveVillainTriggers();
            GoToNextTurn();

            var powers = GetUsablePowersThisTurn(labyrinth);
            Assert.AreEqual(powers.Count(p => p.Index == 1 && p.CardSource.Card == crew), 0);
        }

        [Test()]
        public void TestThirdPower()
        {
            // Select a damage type. Until the start of your next turn whenever damage of that type would be dealt reduce it by 1.[/y]{BR}
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Jp.ParahumansOfTheWormverse.Tattletale", "InsulaPrimalis");

            StartGame();

            var crew = PlayCard("CallInTheCrew");

            var platform = GetMobileDefensePlatform().Card;

            QuickHPStorage(labyrinth.CharacterCard, tattletale.CharacterCard, platform);
            DecisionSelectDamageType = DamageType.Fire;
            UsePower(crew, 2);
            QuickHPCheck(0, 0, 0);
            AssertNumberOfStatusEffectsInPlay(1);
            DealDamage(platform, platform, 2, DamageType.Fire);
            QuickHPCheck(0, 0, -1);
            DealDamage(labyrinth.CharacterCard, labyrinth.CharacterCard, 3, DamageType.Fire);
            QuickHPCheck(-2, 0, 0);
            DealDamage(tattletale.CharacterCard, tattletale.CharacterCard, 4, DamageType.Cold);
            QuickHPCheck(0, -4, 0);

            RemoveVillainTriggers();
            GoToNextTurn();

            var powers = GetUsablePowersThisTurn(labyrinth);
            Assert.AreEqual(powers.Count(p => p.Index == 2 && p.CardSource.Card == crew), 0);
        }

        [Test()]
        public void TestFourthPower()
        {
            // Select a damage type. Until the start of your next turn whenever damage of that type would be dealt reduce it by 1.[/y]{BR}
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Jp.ParahumansOfTheWormverse.Tattletale", "InsulaPrimalis");

            StartGame();

            var crew = PlayCard("CallInTheCrew");

            var platform = GetMobileDefensePlatform().Card;

            QuickHPStorage(platform);
            DecisionSelectTarget = platform;
            UsePower(crew, 3);
            QuickHPCheck(-4);

            RemoveVillainTriggers();
            GoToNextTurn();

            var powers = GetUsablePowersThisTurn(labyrinth);
            Assert.AreEqual(powers.Count(p => p.Index == 3 && p.CardSource.Card == crew), 0);
        }

        [Test()]
        public void TestFifthPower()
        {
            // Select a damage type. Until the start of your next turn whenever damage of that type would be dealt reduce it by 1.[/y]{BR}
            SetupGameController("BaronBlade", "Legacy", "Jp.ParahumansOfTheWormverse.Labyrinth", "Jp.ParahumansOfTheWormverse.Tattletale", "InsulaPrimalis");

            StartGame();

            var crew = PlayCard("CallInTheCrew");

            QuickHandStorage(legacy, labyrinth, tattletale);
            UsePower(crew, 4);
            QuickHandCheck(1, 1, 1);

            RemoveVillainTriggers();
            GoToNextTurn();

            var powers = GetUsablePowersThisTurn(labyrinth);
            Assert.AreEqual(powers.Count(p => p.Index == 4 && p.CardSource.Card == crew), 0);
        }
    }
}
