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
    public class CherishTests : BaseTest
    {
        protected Card cherish { get { return FindCard(c => c.Identifier == "CherishCharacter"); } }

        protected HeroTurnTakerController alexandria { get { return FindHero("Alexandria"); } }

        [Test()]
        public void TestCounterStraightforward()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Megalopolis"
            );

            PlayCard("CherishCharacter");

            QuickHPStorage(alexandria.CharacterCard);

            DealDamage(alexandria, cherish, 2, DamageType.Melee);

            QuickHPCheck(-2);
        }

        [Test()]
        public void TestCounterSelf()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Megalopolis"
            );

            PlayCard("CherishCharacter");

            QuickHPStorage(cherish);

            DealDamage(cherish, cherish, 2, DamageType.Melee);

            QuickHPCheck(-2);
        }

        [Test()]
        public void TestCounterIndestructible()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "TimeCataclysm"
            );

            PlayCard("CherishCharacter");
            PlayCard("FixedPoint");

            SetHitPoints(cherish, 0);

            QuickHPStorage(alexandria.CharacterCard, cherish);

            DealDamage(alexandria, cherish, 2, DamageType.Melee);

            QuickHPCheck(0, -2);
        }

        [Test()]
        public void TestCanBeIncapped()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Megalopolis"
            );

            PlayCard("CherishCharacter");

            DealDamage(cherish, cherish, 30, DamageType.Melee);

            AssertFlipped(cherish);
        }

        [Test()]
        public void TestFlipDamage()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "InsulaPrimalis"
            );

            StartGame();
            RemoveVillainCards();

            PlayCard("CherishCharacter");
            PlayCard("MannequinCharacter");

            GoToStartOfTurn(alexandria);

            DealDamage(cherish, cherish, 100, DamageType.Sonic);

            var raptors = PlayCard("VelociraptorPack");

            QuickHPStorage(alexandria);

            AssertNotDamageSource(cherish);
            DestroyCard(raptors);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestFlipDamageSentinels()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "TheSentinels",
                "InsulaPrimalis"
            );

            StartGame();
            RemoveVillainCards();

            PlayCard("CherishCharacter");
            PlayCard("MannequinCharacter");

            GoToStartOfTurn(sentinels);

            DealDamage(cherish, cherish, 100, DamageType.Sonic);

            var raptors = PlayCard("VelociraptorPack");

            QuickHPStorage(mainstay, medico, idealist, writhe);

            DecisionSelectCard = medico;
            AssertNotDamageSource(cherish);
            DestroyCard(raptors);
            QuickHPCheck(0, -1, 0, 0);
        }
    }
}
