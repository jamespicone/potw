using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Dragon
{
    [TestFixture()]
    public class AzazelTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var card = PlayCard("Azazel");
            AssertIsInPlay(card);
        }

        [Test()]
        public void TestFocusCostOf2()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var tokenPool = dragon.CharacterCard.FindTokenPool("FocusPool");
            var mech = PlayCard("Azazel");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            // Base 4 - 2 from Azazel = 2
            AssertTokenPoolCount(tokenPool, 2);
        }

        [Test()]
        public void TestReducesDamageToDragonBy1()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var mech = PlayCard("Azazel");

            QuickHPStorage(dragon);
            DealDamage(baron, dragon, 3, DamageType.Melee);
            QuickHPCheck(-2); // 3 - 1 = 2
        }

        [Test()]
        public void TestReductionOnlyAppliesToDragon()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Bunker", "InsulaPrimalis");
            StartGame();

            var mech = PlayCard("Azazel");

            // Damage to Bunker should NOT be reduced
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 3, DamageType.Melee);
            QuickHPCheck(-3); // Full damage
        }

        [Test()]
        public void TestEndOfTurn_MayDeal2IrreducibleEnergy()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var mech = PlayCard("Azazel");

            DecisionDoNotActivatableAbility = true;
            DecisionSelectTarget = baron.CharacterCard;
            DecisionYesNo = true;

            QuickHPStorage(baron);
            AssertDamageSource(mech);
            AssertDamageType(DamageType.Energy);
            AssertIrreducible();

            GoToEndOfTurn(dragon);

            QuickHPCheck(-2);
        }

        [Test()]
        public void TestEndOfTurnDamageIsOptional()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var mech = PlayCard("Azazel");

            DecisionDoNotActivatableAbility = true;
            // Skip the optional target selection by not selecting any card
            DecisionDoNotSelectCard = SelectionType.SelectTarget;

            QuickHPStorage(baron);
            GoToEndOfTurn(dragon);
            QuickHPCheck(0); // No damage dealt
        }

        [Test()]
        public void TestFocusAbility_Deal2IrreducibleEnergy()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var mech = PlayCard("Azazel");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            DecisionDoNotActivatableAbility = false;
            DecisionActivateAbilities = new Card[] { mech };
            DecisionSelectTarget = baron.CharacterCard;

            QuickHPStorage(baron);
            AssertDamageSource(mech);
            AssertDamageType(DamageType.Energy);
            AssertIrreducible();

            UsePower(dragon.CharacterCard);

            QuickHPCheck(-2);
        }

        [Test()]
        public void TestDamageIsIrreducible()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Legacy", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var mech = PlayCard("Azazel");

            // Play Heroic Interception to reduce damage
            PlayCard("HeroicInterception");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            DecisionDoNotActivatableAbility = false;
            DecisionActivateAbilities = new Card[] { mech };
            DecisionSelectTarget = baron.CharacterCard;
            AssertIrreducible();

            QuickHPStorage(baron);
            UsePower(dragon.CharacterCard);

            // Damage should be full despite reduction
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestIsDeviceAndMech()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var mech = PlayCard("Azazel");

            Assert.That(mech.DoKeywordsContain("device"), Is.True, "Azazel should be a Device");
            Assert.That(mech.DoKeywordsContain("mech"), Is.True, "Azazel should be a Mech");
        }

        [Test()]
        public void TestHas18HP()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var mech = PlayCard("Azazel");

            Assert.That(mech.MaximumHitPoints, Is.EqualTo(18));
        }

        [Test()]
        public void TestReductionStillAppliesWithOtherMechs()
        {
            // Azazel only has 1 copy, but we can test that it still works with other mechs
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            PlayCard("Azazel");
            PlayCard("Ladon"); // Another mech

            // Damage to Dragon should be reduced by 1 from Azazel
            QuickHPStorage(dragon);
            DealDamage(baron, dragon, 4, DamageType.Melee);
            QuickHPCheck(-3); // 4 - 1 = 3
        }
    }
}
