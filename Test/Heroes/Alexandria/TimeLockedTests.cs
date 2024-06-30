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
    public class TimeLockedTests : ParahumanTest
    {
        // todo test promo swap?
        [Test()]
        public void TestWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            QuickHPStorage(alexandria);
            AssertNumberOfStatusEffectsInPlay(0);
            GoToPlayCardPhaseAndPlayCard(alexandria, "TimeLocked");
            AssertNumberOfStatusEffectsInPlay(1);
            QuickHPCheck(0);

            DealDamage(baron, alexandria, 10, DamageType.Toxic);
            AssertNumberOfStatusEffectsInPlay(1);
            QuickHPCheck(-10);

            GoToEndOfTurn(baron);
            AssertNumberOfStatusEffectsInPlay(1);
            QuickHPCheck(0);

            GoToStartOfTurn(alexandria);
            AssertNumberOfStatusEffectsInPlay(0);
            QuickHPCheck(10);
        }

        [Test()]
        public void TestAlexIncapped()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "Legacy", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            QuickHPStorage(alexandria);
            AssertNumberOfStatusEffectsInPlay(0);
            GoToPlayCardPhaseAndPlayCard(alexandria, "TimeLocked");
            AssertNumberOfStatusEffectsInPlay(1);
            IncapacitateCharacter(alexandria.CharacterCard, alexandria.CharacterCard);
            AssertNumberOfStatusEffectsInPlay(0);
        }

        [Test()]
        public void TestPlayMultipleTimes()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            QuickHPStorage(alexandria);
            AssertNumberOfStatusEffectsInPlay(0);
            var timelocked = GoToPlayCardPhaseAndPlayCard(alexandria, "TimeLocked");
            AssertNumberOfStatusEffectsInPlay(1);
            DealDamage(alexandria, alexandria, 10, DamageType.Infernal);
            PlayCard(timelocked);
            AssertNumberOfStatusEffectsInPlay(2);
            QuickHPCheck(-10);

            GoToEndOfTurn(baron);
            AssertNumberOfStatusEffectsInPlay(2);
            QuickHPCheck(0);

            GoToStartOfTurn(alexandria);
            AssertNumberOfStatusEffectsInPlay(0);
            // They resolve in order; so alexandria's HP goes to 30 and then 20.
            QuickHPCheck(0);
        }

        [Test()]
        public void TestWorksOnStartPhase()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            QuickHPStorage(alexandria);
            AssertNumberOfStatusEffectsInPlay(0);
            GoToStartOfTurn(alexandria);
            PlayCard("TimeLocked");
            AssertNumberOfStatusEffectsInPlay(1);
            QuickHPCheck(0);

            DealDamage(baron, alexandria, 10, DamageType.Toxic);
            AssertNumberOfStatusEffectsInPlay(1);
            QuickHPCheck(-10);

            GoToEndOfTurn(baron);
            AssertNumberOfStatusEffectsInPlay(1);
            QuickHPCheck(0);

            GoToStartOfTurn(alexandria);
            AssertNumberOfStatusEffectsInPlay(0);
            QuickHPCheck(10);
        }

        [Test()]
        public void TestWorksOffTurn()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            QuickHPStorage(alexandria);
            AssertNumberOfStatusEffectsInPlay(0);
            GoToEndOfTurn(baron);
            PlayCard("TimeLocked");
            AssertNumberOfStatusEffectsInPlay(1);
            QuickHPCheck(0);

            DealDamage(baron, alexandria, 10, DamageType.Toxic);
            AssertNumberOfStatusEffectsInPlay(1);
            QuickHPCheck(-10);

            GoToStartOfTurn(alexandria);
            AssertNumberOfStatusEffectsInPlay(0);
            QuickHPCheck(10);
        }
    }
}
