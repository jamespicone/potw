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
    public class BonesawTests : Slaughterhouse9TestBase
    {
        [Test()]
        public void TestCanBeIncapped()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Megalopolis"
            );

            PutMemberInPlay("BonesawCharacter");

            DealDamage(bonesaw, bonesaw, 30, DamageType.Melee);

            AssertFlipped(bonesaw);
        }

        [Test()]
        public void TestDefenceHealsVillainTargets()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Megalopolis"
            );

            PutMemberInPlay("BonesawCharacter");
            DealDamage(bonesaw, bonesaw, 5, DamageType.Melee);

            var lessThanHuman = PlayCard("LessThanHuman");

            QuickHPStorage(bonesaw);
            DestroyCard(lessThanHuman);
            QuickHPCheck(2);
        }

        [Test()]
        public void TestSpecialDamagesAllNonVillainTargets()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Megalopolis"
            );

            PutMemberInPlay("BonesawCharacter");

            var legendary = PlayCard("Legendary");

            QuickHPStorage(alexandria.CharacterCard, bonesaw);
            DestroyCard(legendary);
            QuickHPCheck(-2, 0);
        }

        [Test()]
        public void TestFlippedHealsTheLowestVillainTargetAtEndOfVillainTurn()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Megalopolis"
            );

            StartGame();
            PutMemberInPlay("BonesawCharacter");
            ReturnMembersExcept(bonesaw);

            // A second villain target so flipping Bonesaw doesn't end the game.
            var spider = PlayCard("Spiderbots");
            RemoveVillainDeck();

            DealDamage(bonesaw, bonesaw, 100, DamageType.Melee);
            AssertFlipped(bonesaw);

            SetHitPoints(spider, 2);

            QuickHPStorage(spider);
            GoToEndOfTurn(nine);
            QuickHPCheck(1);
        }
    }
}
