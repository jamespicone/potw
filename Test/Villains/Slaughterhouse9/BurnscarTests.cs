using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Jp.ParahumansOfTheWormverse.Tattletale;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Slaughterhouse9
{
    [TestFixture()]
    public class BurnscarTests : Slaughterhouse9TestBase
    {
        [Test()]
        public void TestCanBeIncapped()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Megalopolis"
            );

            PutMemberInPlay("BurnscarCharacter");

            DealDamage(burnscar, burnscar, 30, DamageType.Melee);

            AssertFlipped(burnscar);
        }

        [Test()]
        public void TestSpecialDestroysTheEnvironmentAndBurnsForEachCardDestroyed()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Megalopolis"
            );

            PutMemberInPlay("BurnscarCharacter");

            var pileup = PlayCard("TrafficPileup");

            var legendary = PlayCard("Legendary");

            QuickHPStorage(alexandria.CharacterCard, burnscar);
            DestroyCard(legendary);

            AssertInTrash(pileup);
            // X = 1 + the 1 environment card destroyed.
            QuickHPCheck(-2, 0);
        }

        [Test()]
        public void TestAttackBurnsTheLowestHeroAndDestroysACard()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Megalopolis"
            );

            PutMemberInPlay("BurnscarCharacter");

            var cape = PlayCard("AlexandriasCape");

            var prisoners = PlayCard("WeDontTakePrisoners");

            DecisionSelectCard = cape;
            QuickHPStorage(alexandria.CharacterCard);
            DestroyCard(prisoners);

            QuickHPCheck(-3);
            AssertInTrash(cape);
        }

        [Test()]
        public void TestFlippedBurnsAllNonVillainTargetsAtEndOfVillainTurn()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Megalopolis"
            );

            StartGame();
            PutMemberInPlay("BurnscarCharacter");
            ReturnMembersExcept(burnscar);

            // A second villain target so flipping Burnscar doesn't end the game.
            // Hatchet Face has no end-of-turn behaviour of his own.
            PlayCard("HatchetFace");
            RemoveVillainDeck();

            DealDamage(burnscar, burnscar, 100, DamageType.Melee);
            AssertFlipped(burnscar);

            QuickHPStorage(alexandria.CharacterCard);
            GoToEndOfTurn(nine);
            QuickHPCheck(-1);
        }
    }
}
