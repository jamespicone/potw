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
    public class AnalysisTests : ParahumanTest
    {
        [Test()]
        public void TestIncreasesDragonDamageBy1()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            PlayCard("Analysis");

            QuickHPStorage(baron);
            DealDamage(dragon, baron, 2, DamageType.Melee);
            QuickHPCheck(-3); // 2 + 1 = 3
        }

        [Test()]
        public void TestIncreasesMechDamageBy1()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var mech = PlayCard("Ladon");
            PlayCard("Analysis");

            QuickHPStorage(baron);
            DealDamage(mech, baron, 2, DamageType.Melee);
            QuickHPCheck(-3); // 2 + 1 = 3
        }

        [Test()]
        public void TestDoesNotIncreaseOtherHeroDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            PlayCard("Analysis");

            // Bunker's damage should NOT be increased
            QuickHPStorage(baron);
            DealDamage(bunker, baron, 2, DamageType.Melee);
            QuickHPCheck(-2); // Full damage, no increase
        }

        [Test()]
        public void TestExpiresAtEndOfTurn()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            PlayCard("Analysis");

            // Damage increase should be active
            QuickHPStorage(baron);
            DealDamage(dragon, baron, 2, DamageType.Melee);
            QuickHPCheck(-3); // 2 + 1 = 3

            // Go to end of turn
            GoToEndOfTurn(dragon);

            // Damage increase should be gone
            QuickHPStorage(baron);
            DealDamage(dragon, baron, 2, DamageType.Melee);
            QuickHPCheck(-2); // Normal damage
        }

        [Test()]
        public void TestMultipleAnalysisStack()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            PlayCard("Analysis", 0);
            PlayCard("Analysis", 1);

            QuickHPStorage(baron);
            DealDamage(dragon, baron, 2, DamageType.Melee);
            QuickHPCheck(-4); // 2 + 1 + 1 = 4
        }

        [Test()]
        public void TestIsOneShot()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var card = GetCard("Analysis");

            Assert.That(card.IsOneShot, Is.True, "Analysis should be a One-Shot");
        }

        [Test()]
        public void TestOnlyMechsInDragonPlayAreaGetBoost()
        {
            // If another card controller somehow puts a mech in a different play area,
            // it shouldn't get the boost
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            // Play a mech - should be in Dragon's play area
            var mech = PlayCard("Ladon");
            Assert.That(mech.Location, Is.EqualTo(dragon.TurnTaker.PlayArea));

            PlayCard("Analysis");

            // Mech should get the damage boost
            QuickHPStorage(baron);
            DealDamage(mech, baron, 2, DamageType.Melee);
            QuickHPCheck(-3); // 2 + 1 = 3
        }
    }
}
