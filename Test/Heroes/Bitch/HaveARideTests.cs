using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Bitch
{
    [TestFixture()]
    public class HaveARideTests : ParahumanTest
    {
        [Test()]
        public void TestReducesEnvironmentDamageToHeroes()
        {
            // Arrange
            SetupGameController("Jp.ParahumansOfTheWormverse.Bitch", "Legacy", "Tachyon", "Jp.ParahumansOfTheWormverse.Battery", "InsulaPrimalis");

            StartGame();

            PlayCard("HaveARide");

            // Track HP of all hero targets
            QuickHPStorage(bitch, legacy, tachyon, battery);

            // Deal damage from environment card
            DecisionDoNotSelectCard = SelectionType.DestroyCard;
            PlayCard("PrimordialPlantLife");
            
            // Should do 3 damage to each hero instead of 4
            QuickHPCheck(-3, -3, -3, -3);
        }

        [Test()]
        public void TestOnlyReducesEnvironmentDamage()
        {
            // Arrange
            SetupGameController("Jp.ParahumansOfTheWormverse.Bitch", "BaronBlade", "InsulaPrimalis");

            StartGame();

            PlayCard("HaveARide");

            // Track HP of all hero targets
            QuickHPStorage(bitch);

            // Deal damage from villain card (should not be reduced)
            DealDamage(baron, bitch, 3, DamageType.Melee);
            QuickHPCheck(-3); // Full damage taken

            // Deal damage from environment card (should be reduced)
            var dino = PlayCard("VelociraptorPack");
            DealDamage(dino, bitch, 2, DamageType.Melee);
            QuickHPCheck(-1); // Reduced by 1
        }

        [Test()]
        public void TestOnlyReducesDamageToHeroes()
        {
            // Arrange  
            SetupGameController("Jp.ParahumansOfTheWormverse.Bitch", "Legacy", "BaronBlade", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            Card ride = PlayCard("HaveARide");

            // Track HP of hero and villain targets
            QuickHPStorage(legacy, baron);

            // Deal environment damage to both
            var trex = PlayCard("EnragedTRex");
            DealDamage(trex, new Card[] { baron.CharacterCard, legacy.CharacterCard }, 5, DamageType.Melee);

            // Hero damage should be reduced, villain damage should not
            QuickHPCheck(-4, -5);
        }

        [Test()]
        public void TestDestroyHaveARide()
        {
            // Arrange
            SetupGameController("Jp.ParahumansOfTheWormverse.Bitch", "BaronBlade", "InsulaPrimalis");

            StartGame();

            Card ride = PlayCard("HaveARide");
            QuickHPStorage(bitch);

            // Deal environment damage - should be reduced
            var trex = PlayCard("EnragedTRex");

            DealDamage(trex, bitch, 5, DamageType.Psychic);
            QuickHPCheck(-4); // Reduced from 5 to 4

            // Destroy Have a Ride
            DestroyCard(ride);

            // Deal environment damage again - should not be reduced
            DealDamage(trex, bitch, 5, DamageType.Psychic);
            QuickHPCheck(-5); // Full damage
        }
    }
}