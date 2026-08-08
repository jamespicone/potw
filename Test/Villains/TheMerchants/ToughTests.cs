using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.TheMerchants
{
    [TestFixture()]
    public class ToughTests : ParahumanTest
    {
        [Test()]
        public void HitsHighest()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "Legacy", "Parse", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            var tough = PlayCard("Tough");

            SetHitPoints(tempest, 15);
            SetHitPoints(legacy, 16);
            SetHitPoints(parse, 14);
            QuickHPStorage(parse, tempest, legacy);
            AssertDamageSource(tough);
            AssertDamageType(DamageType.Melee);
            GoToEndOfTurn();
            QuickHPCheck(0, -2, -2);
        }

        [Test()]
        public void HitsSingleTargetOnceWhenOnlyOneHeroTarget()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "Legacy", "Parse", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            var tough = PlayCard("Tough");

            DealDamage(merchants, legacy, 50, DamageType.Infernal);
            DealDamage(merchants, parse, 50, DamageType.Infernal);

            QuickHPStorage(tempest);
            AssertDamageSource(tough);
            AssertDamageType(DamageType.Melee);
            GoToEndOfTurn();
            QuickHPCheck(-2);
        }

        [Test()]
        public void ShufflesIntoThugDeckWhenDestroyed()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "Legacy", "Parse", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            var tough = PlayCard("Tough");

            DestroyCard(tough, tempest.CharacterCard);

            AssertNotInPlay(tough);
            AssertAtLocation(tough, merchants.TurnTaker.FindSubDeck("ThugDeck"));
        }
    }
}
