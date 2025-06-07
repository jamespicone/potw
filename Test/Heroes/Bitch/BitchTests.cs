using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Bitch
{
    [TestFixture()]
    public class BitchTests : ParahumanTest
    {
        [Test()]
        public void TestPowerNoDogs()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            QuickHPStorage(baron.CharacterCard, bitch.CharacterCard);

            UsePower(bitch.CharacterCard);

            QuickHPCheck(0, 0);
        }

        [Test()]
        public void TestPowerOneDog()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            var bastard = PlayCard("Bastard");

            QuickHPStorage(baron.CharacterCard, bitch.CharacterCard);

            AssertDamageType(DamageType.Melee);
            AssertDamageSource(bastard);
            DecisionSelectTarget = baron.CharacterCard;
            UsePower(bitch.CharacterCard);

            QuickHPCheck(-2, 0);
        }

        [Test()]
        public void TestPowerTwoDog()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            var bastard = PlayCard("Bastard");
            var milk = PlayCard("Milk");

            QuickHPStorage(baron.CharacterCard, bitch.CharacterCard);

            AssertDamageType(DamageType.Melee, DamageType.Melee);
            AssertDamageSource(milk, bastard);
            DecisionSelectCards = new Card[] { milk, baron.CharacterCard, bitch.CharacterCard };
            UsePower(bitch.CharacterCard);

            QuickHPCheck(-2, -2);
        }

        [Test()]
        public void TestIncapFirstAbilityHealsTarget()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "Legacy", "InsulaPrimalis");
            StartGame();

            // Set legacy's HP lower so we can heal
            SetHitPoints(legacy, 20);

            // Get Bitch incapacitated
            DealDamage(baron, bitch, 30, DamageType.Melee);
            AssertIncapacitated(bitch);

            // Track HP changes
            QuickHPStorage(legacy);

            // Use first incap ability, selecting Legacy to heal
            DecisionSelectCard = legacy.CharacterCard;
            UseIncapacitatedAbility(bitch, 0);

            // Check that Legacy gained 2 HP
            QuickHPCheck(2);
        }

        [Test()]
        public void TestIncapSecondAbilityMovesCardFromTrashToHand()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "Legacy", "InsulaPrimalis");
            StartGame();

            // Get Bitch incapacitated
            DealDamage(baron, bitch, 30, DamageType.Melee);
            AssertIncapacitated(bitch);

            // Put a card in Legacy's trash
            var motivationalCharge = PutInTrash("MotivationalCharge");
            AssertInTrash(motivationalCharge);

            // Use second incap ability
            DecisionSelectTurnTaker = legacy.TurnTaker;
            DecisionSelectCard = motivationalCharge;
            UseIncapacitatedAbility(bitch, 1);

            // Verify card moved from trash to hand
            AssertInHand(motivationalCharge);
        }

        [Test()]
        public void TestIncapThirdAbilityReducesEnvironmentDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "Legacy", "InsulaPrimalis");
            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            // Get Bitch incapacitated
            DealDamage(baron, bitch, 30, DamageType.Melee);
            AssertIncapacitated(bitch);

            // Get an environment card to deal damage
            Card raptor = PlayCard("VelociraptorPack");
            QuickHPStorage(legacy);

            // Use third incap ability 
            UseIncapacitatedAbility(bitch, 2);

            // Deal damage with environment card - should be reduced by 2
            DealDamage(raptor, legacy, 3, DamageType.Melee);
            QuickHPCheck(-1); // 3 damage reduced by 2

            // Go to start of Bitch's next turn
            GoToStartOfTurn(bitch);

            // Damage reduction should be gone now
            QuickHPStorage(legacy);
            DealDamage(raptor, legacy, 3, DamageType.Melee);
            QuickHPCheck(-3); // Full damage
        }
    }
}
