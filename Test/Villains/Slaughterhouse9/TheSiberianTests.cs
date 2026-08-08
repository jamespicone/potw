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
    public class TheSiberianTests : Slaughterhouse9TestBase
    {
        [Test()]
        public void TestCanBeIncapped()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Megalopolis"
            );

            PutMemberInPlay("TheSiberianCharacter");

            DealDamage(siberian, siberian, 30, DamageType.Melee);

            AssertFlipped(siberian);
        }

        [Test()]
        public void TestMembersPlayedJustBeforeAndAfterTheSiberianAreImmune()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Megalopolis"
            );

            PutMemberInPlay("BonesawCharacter");
            PutMemberInPlay("TheSiberianCharacter");
            PutMemberInPlay("CrawlerCharacter");
            PutMemberInPlay("MannequinCharacter");

            QuickHPStorage(bonesaw, crawler, mannequin, siberian);

            DealDamage(alexandria, bonesaw, 2, DamageType.Melee);
            DealDamage(alexandria, crawler, 2, DamageType.Melee);
            DealDamage(alexandria, mannequin, 3, DamageType.Melee);
            DealDamage(alexandria, siberian, 2, DamageType.Melee);

            // Bonesaw and Crawler are adjacent to The Siberian and immune;
            // Mannequin (reducing by 1) and The Siberian herself are not.
            QuickHPCheck(0, 0, -2, -2);
        }

        [Test()]
        public void TestFlippedMakesTheLowestVillainTargetImmuneToMelee()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Megalopolis"
            );

            StartGame();
            PutMemberInPlay("TheSiberianCharacter");
            ReturnMembersExcept(siberian);

            var spider = PlayCard("Spiderbots");

            DealDamage(siberian, siberian, 100, DamageType.Melee);
            AssertFlipped(siberian);

            QuickHPStorage(spider);

            DealDamage(alexandria, spider, 1, DamageType.Melee);
            QuickHPCheck(0);

            DealDamage(alexandria, spider, 1, DamageType.Fire);
            QuickHPCheck(-1);
        }
    }
}
