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
    public class ShatterbirdTests : Slaughterhouse9TestBase
    {
        [Test()]
        public void TestCanBeIncapped()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Megalopolis"
            );

            PutMemberInPlay("ShatterbirdCharacter");

            DealDamage(shatterbird, shatterbird, 30, DamageType.Melee);

            AssertFlipped(shatterbird);
        }

        [Test()]
        public void TestAttackHitsTheHighestHeroTargets()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Legacy",
                "Bunker",
                "Haka",
                "Megalopolis"
            );

            PutMemberInPlay("ShatterbirdCharacter");

            SetHitPoints(legacy, 25);
            SetHitPoints(bunker, 10);
            SetHitPoints(haka, 20);

            var prisoners = PlayCard("WeDontTakePrisoners");

            QuickHPStorage(legacy.CharacterCard, bunker.CharacterCard, haka.CharacterCard);
            DestroyCard(prisoners);

            // H - 1 = the 2 highest hero targets take 2 projectile.
            QuickHPCheck(-2, 0, -2);
        }

        [Test()]
        public void TestFlippedHitsAllNonVillainTargetsAtEndOfVillainTurn()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Megalopolis"
            );

            StartGame();
            PutMemberInPlay("ShatterbirdCharacter");
            ReturnMembersExcept(shatterbird);

            // A second villain target so flipping Shatterbird doesn't end the game.
            // Hatchet Face has no end-of-turn behaviour of his own.
            PlayCard("HatchetFace");
            RemoveVillainDeck();

            DealDamage(shatterbird, shatterbird, 100, DamageType.Melee);
            AssertFlipped(shatterbird);

            QuickHPStorage(alexandria.CharacterCard);
            GoToEndOfTurn(nine);
            QuickHPCheck(-1);
        }
    }
}
