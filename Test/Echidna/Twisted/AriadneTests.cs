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
    public class AriadneTests : EchidnaBaseTest
    {
        protected HeroTurnTakerController alexandria { get { return FindHero("Alexandria"); } }
        protected HeroTurnTakerController bitch { get { return FindHero("Bitch"); } }

        protected HeroTurnTakerController legend { get { return FindHero("Legend"); } }

        [Test()]
        public void TestPlaysEnvironmentCard()
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

            StackDeck("PsychologicalWarfare");
            var field = StackDeck("ObsidianField");
            var raptors = StackDeck("VelociraptorPack");

            PlayCard("AriadneTwisted");

            AssertInDeck(raptors);
            AssertInDeck(field);

            GoToEndOfTurn(echidna);

            AssertIsInPlay(raptors);
            AssertInDeck(field);
        }

        [Test()]
        public void TestPreventsDamage()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "PikeIndustrialComplex"
            );

            StartGame();
            DestroyNonCharacterVillainCards();
            ReturnAllTwisted();

            StackDeck("PsychologicalWarfare");
            StackDeck("ChemicalExplosion");

            var ariadne = PlayCard("AriadneTwisted");

            QuickHPStorage(echidna.CharacterCard, ariadne);
            GoToEndOfTurn(echidna);
            EnterNextTurnPhase();

            // Immune to damage
            QuickHPCheck(0, 0);
        }

        [Test()]
        public void TestPreventsDestroy()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "ChampionStudios"
            );

            StartGame();
            DestroyNonCharacterVillainCards();
            ReturnAllTwisted();

            var ariadne = PlayCard("AriadneTwisted");
            PlayCard("CarChaseScene");

            DealDamage(alexandria, ariadne, 9, DamageType.Radiant);

            AssertIsInPlay(ariadne);
            AssertHitPoints(ariadne, 1);
        }

        [Test()]
        public void TestDoesntPreventSelfDestroy()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "Jp.ParahumansOfTheWormverse.BrocktonBay"
            );

            StartGame();
            DestroyNonCharacterVillainCards();
            ReturnAllTwisted();
            StackDeck("PsychologicalWarfare");

            GoToDrawCardPhase(alexandria);

            PlayCard("AriadneTwisted");
            var bunker = PlayCard("Rooftops");

            DecisionYesNo = true;
            EnterNextTurnPhase();

            AssertNotInPlay(bunker);
        }
    }
}
