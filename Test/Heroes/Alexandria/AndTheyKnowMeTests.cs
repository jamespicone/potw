using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Alexandria
{
    [TestFixture()]
    public class AndTheyKnowMeTests : ParahumanTest
    {
        [Test()]
        public void TestWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "InsulaPrimalis");

            StartGame();

            RemoveVillainTriggers();
            RemoveVillainCards();

            AssertDamageSource(baron.CharacterCard);
            AssertDamageType(DamageType.Psychic);

            QuickHPStorage(alexandria.CharacterCard);
            AssertNumberOfStatusEffectsInPlay(0);
            PlayCard("AndTheyKnowMe");
            QuickHPCheck(-5);
            AssertNumberOfStatusEffectsInPlay(1);

            QuickHPStorage(alexandria.CharacterCard, baron.CharacterCard);
            
            // Can't hurt alex
            DealDamage(baron, alexandria, 2, DamageType.Infernal);
            QuickHPCheck(0, 0);

            // Can't hurt himself
            DealDamage(baron, baron, 2, DamageType.Infernal);
            QuickHPCheck(0, 0);
        }

        [Test()]
        public void TestOnlyRestrictsHighestHPVillainTarget()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "InsulaPrimalis");

            StartGame();

            RemoveVillainTriggers();

            // Baron Blade (40 HP) outranks the Mobile Defense Platform (10 HP).
            var mdp = GetCardInPlay("MobileDefensePlatform");

            QuickHPStorage(alexandria.CharacterCard);
            PlayCard("AndTheyKnowMe");
            QuickHPCheck(-5);

            // Baron is barred from dealing damage, but the platform isn't.
            QuickHPStorage(alexandria.CharacterCard);
            DealDamage(baron.CharacterCard, alexandria.CharacterCard, 2, DamageType.Melee);
            QuickHPCheck(0);

            QuickHPStorage(alexandria.CharacterCard);
            DealDamage(mdp, alexandria.CharacterCard, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestCannotDealDamageExpiresAtStartOfAlexandriasTurn()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "InsulaPrimalis");

            StartGame();

            RemoveVillainTriggers();
            RemoveVillainCards();

            PlayCard("AndTheyKnowMe");
            AssertNumberOfStatusEffectsInPlay(1);

            GoToStartOfTurn(alexandria);
            AssertNumberOfStatusEffectsInPlay(0);

            QuickHPStorage(alexandria.CharacterCard);
            DealDamage(baron, alexandria, 2, DamageType.Infernal);
            QuickHPCheck(-2);
        }
    }
}
