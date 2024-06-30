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
    public class SupersonicFlightTests : ParahumanTest
    {
        [Test()]
        public void TestDealsDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            QuickHPStorage(baron);

            AssertDamageSource(alexandria.CharacterCard);
            AssertDamageType(DamageType.Sonic);
            DecisionSelectTarget = baron.CharacterCard;
            PlayCard("SupersonicFlight");
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestCanDestroyDevices()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            DecisionSelectTargets = new Card[] { null };

            PlayCard("SupersonicFlight");
            var platform = PlayCard("MobileDefensePlatform");

            ResetDecisions();
            DecisionSelectCard = platform;

            DealDamage(alexandria, platform, 1, DamageType.Fire);
            AssertInTrash(platform);
        }

        [Test()]
        public void TestCanDestroyOngoings()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            DecisionSelectTargets = new Card[] { null };

            PlayCard("SupersonicFlight");
            var forcefield = PlayCard("LivingForceField");

            ResetDecisions();
            DecisionSelectCard = forcefield;

            DealDamage(alexandria, baron, 2, DamageType.Fire);
            AssertInTrash(forcefield);
        }

        [Test()]
        public void TestCantDestroyCharacterDevices()
        {
            SetupGameController("Omnitron", "Jp.ParahumansOfTheWormverse.Alexandria", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            DecisionSelectTargets = new Card[] { null };

            var flight = PlayCard("SupersonicFlight");

            AssertNextDecisionChoices(
                included: new Card[] { flight },
                notIncluded: new Card[] { omnitron.CharacterCard }
            );
            DealDamage(alexandria, omnitron, 1, DamageType.Melee);
        }
    }
}
