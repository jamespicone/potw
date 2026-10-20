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

        // Hurricane Shield overrides CanOrderAffectOutcome, so the engine simulates the
        // other reduction triggers to see if their order matters. That simulation must not
        // rerun Mannequin's tied-for-lowest question.
        [Test()]
        public void TestFlippedTieForLowestIsAskedOnceWithHurricaneShield()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "MaerynianRefuge"
            );

            StartGame();
            PutMemberInPlay("MannequinCharacter");
            ReturnMembersExcept(mannequin);

            var rat = PlayCard("MurderRat");
            var hatchet = PlayCard("HatchetFace");

            DealDamage(alexandria, mannequin, 100, DamageType.Melee);
            AssertFlipped(mannequin);

            // After the targets, so its enters-play damage doesn't hit them.
            PlayCard("HurricaneShield");

            int tieQuestions = 0;
            GameControllerDecisionEvent decider = (IDecision decision) =>
            {
                if (decision is YesNoCardDecision yesNo && yesNo.SelectionType == SelectionType.LowestHP)
                {
                    tieQuestions++;
                }
                return MakeDecisions(decision);
            };

            DecisionYesNo = true;
            QuickHPStorage(rat, hatchet);

            ReplaceOnMakeDecisions(decider);
            DealDamage(alexandria, rat, 6, DamageType.Melee);
            RestoreOnMakeDecisions(decider);

            // 6 - 2 (Hurricane Shield) - 1 (Mannequin), in either order.
            QuickHPCheck(-3, 0);
            Assert.That(tieQuestions, Is.EqualTo(1));
        }
    }
}
