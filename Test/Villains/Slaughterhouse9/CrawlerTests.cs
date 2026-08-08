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
    public class CrawlerTests : Slaughterhouse9TestBase
    {
        [Test()]
        public void TestCanBeIncapped()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Megalopolis"
            );

            PutMemberInPlay("CrawlerCharacter");

            DealDamage(crawler, crawler, 30, DamageType.Melee);

            AssertFlipped(crawler);
        }

        [Test()]
        public void TestBecomesImmuneToTheLastDamageTypeTaken()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Megalopolis"
            );

            // The immunity is tracked via card properties, which need a
            // started game's journal to record.
            StartGame();
            PutMemberInPlay("CrawlerCharacter");
            ReturnMembersExcept(crawler);

            QuickHPStorage(crawler);

            DealDamage(alexandria, crawler, 2, DamageType.Fire);
            QuickHPCheck(-2);

            // Immune to fire now.
            DealDamage(alexandria, crawler, 2, DamageType.Fire);
            QuickHPCheck(0);

            // A new damage type gets through and replaces the immunity.
            DealDamage(alexandria, crawler, 2, DamageType.Melee);
            QuickHPCheck(-2);

            DealDamage(alexandria, crawler, 2, DamageType.Fire);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestAttackHitsTheHighestNonVillainTarget()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Megalopolis"
            );

            PutMemberInPlay("CrawlerCharacter");

            var prisoners = PlayCard("WeDontTakePrisoners");

            QuickHPStorage(alexandria.CharacterCard);
            DestroyCard(prisoners);

            // 2 toxic + 2 melee.
            QuickHPCheck(-4);
        }

        [Test()]
        public void TestSpecialRegains30HP()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Megalopolis"
            );

            PutMemberInPlay("CrawlerCharacter");
            DealDamage(alexandria, crawler, 20, DamageType.Melee);

            var legendary = PlayCard("Legendary");

            QuickHPStorage(crawler);
            DestroyCard(legendary);

            // Capped at his 30 maximum HP.
            QuickHPCheck(20);
        }

        [Test()]
        public void TestFlippedDealsDamageToTheLowestHeroAtEndOfVillainTurn()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Megalopolis"
            );

            StartGame();
            PutMemberInPlay("CrawlerCharacter");
            ReturnMembersExcept(crawler);

            // A second villain target so flipping Crawler doesn't end the game.
            // Hatchet Face has no end-of-turn behaviour of his own.
            PlayCard("HatchetFace");
            RemoveVillainDeck();

            DealDamage(crawler, crawler, 100, DamageType.Melee);
            AssertFlipped(crawler);

            QuickHPStorage(alexandria.CharacterCard);
            GoToEndOfTurn(nine);
            QuickHPCheck(-2);
        }
    }
}
