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
    public class MannequinTests : Slaughterhouse9TestBase
    {
        [Test()]
        public void TestCanBeIncapped()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Megalopolis"
            );

            PutMemberInPlay("MannequinCharacter");

            DealDamage(mannequin, mannequin, 30, DamageType.Melee);

            AssertFlipped(mannequin);
        }

        [Test()]
        public void TestReducesDamageDealtToMannequin()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Megalopolis"
            );

            PutMemberInPlay("MannequinCharacter");

            QuickHPStorage(mannequin);
            DealDamage(alexandria, mannequin, 3, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestDefenceDestroysAHeroOngoingOrEquipmentCard()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Megalopolis"
            );

            PutMemberInPlay("MannequinCharacter");

            var cape = PlayCard("AlexandriasCape");

            var lessThanHuman = PlayCard("LessThanHuman");

            DecisionSelectCard = cape;
            DestroyCard(lessThanHuman);

            AssertInTrash(cape);
        }

        [Test()]
        public void TestFlippedReducesDamageDealtToTheLowestVillainTarget()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Megalopolis"
            );

            StartGame();
            PutMemberInPlay("MannequinCharacter");
            ReturnMembersExcept(mannequin);

            var spider = PlayCard("Spiderbots");

            DealDamage(alexandria, mannequin, 100, DamageType.Melee);
            AssertFlipped(mannequin);

            QuickHPStorage(spider);
            DealDamage(alexandria, spider, 2, DamageType.Melee);
            QuickHPCheck(-1);
        }
    }
}
