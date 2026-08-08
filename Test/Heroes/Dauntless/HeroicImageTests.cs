using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Dauntless
{
    [TestFixture()]
    public class HeroicImageTests : ParahumanTest
    {
        [Test()]
        public void TestHealsDauntless()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "InsulaPrimalis");
            StartGame();

            SetHitPoints(dauntless, 20);

            QuickHPStorage(dauntless);
            DecisionSelectCard = bunker.CharacterCard;
            PlayCard("HeroicImage");

            QuickHPCheck(2);
        }

        [Test()]
        public void TestRedirectsSelectedHeroDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "InsulaPrimalis");
            StartGame();

            DecisionSelectCard = bunker.CharacterCard;
            PlayCard("HeroicImage");

            QuickHPStorage(dauntless, bunker);
            DealDamage(baron, bunker, 3, DamageType.Melee);

            // Damage redirected to Dauntless
            QuickHPCheck(-3, 0);
        }

        [Test()]
        public void TestDoesNotRedirectOtherHeroDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "Tachyon", "InsulaPrimalis");
            StartGame();

            DecisionSelectCard = bunker.CharacterCard;
            PlayCard("HeroicImage");

            QuickHPStorage(dauntless, tachyon);
            DealDamage(baron, tachyon, 3, DamageType.Melee);

            // Damage to Tachyon is not redirected
            QuickHPCheck(0, -3);
        }

        [Test()]
        public void TestRedirectExpiresAtStartOfNextTurn()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "InsulaPrimalis");
            StartGame();

            DecisionSelectCard = bunker.CharacterCard;
            PlayCard("HeroicImage");

            // Go to start of Dauntless's next turn
            GoToStartOfTurn(dauntless);

            // Redirect should be expired
            QuickHPStorage(dauntless, bunker);
            DealDamage(baron, bunker, 3, DamageType.Melee);

            // Damage goes to Bunker (no longer redirected)
            QuickHPCheck(0, -3);
        }

        [Test()]
        public void TestRedirectWorksUntilStartOfNextTurn()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "InsulaPrimalis");
            StartGame();

            DecisionSelectCard = bunker.CharacterCard;
            PlayCard("HeroicImage");

            // Multiple attacks should redirect
            QuickHPStorage(dauntless, bunker);
            DealDamage(baron, bunker, 2, DamageType.Melee);
            DealDamage(baron, bunker, 2, DamageType.Melee);

            QuickHPCheck(-4, 0);
        }

        [Test()]
        public void TestNoRedirectForZeroDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "InsulaPrimalis");
            StartGame();

            // Play a card that reduces damage
            PlayCard("HeavyPlating");

            DecisionSelectCard = bunker.CharacterCard;
            PlayCard("HeroicImage");

            QuickHPStorage(dauntless, bunker);
            DealDamage(baron, bunker, 1, DamageType.Melee);

            // No damage dealt (reduced to 0), so nothing to redirect
            QuickHPCheck(0, 0);
        }

        [Test()]
        public void TestDoesNotRedirectDamageToDauntless()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "InsulaPrimalis");
            StartGame();

            DecisionSelectCard = bunker.CharacterCard;
            PlayCard("HeroicImage");

            // Direct damage to Dauntless
            QuickHPStorage(dauntless);
            DealDamage(baron, dauntless, 3, DamageType.Melee);

            QuickHPCheck(-3); // Direct damage
        }
    }
}
