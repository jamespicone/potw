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
    public class PureStrengthTests : ParahumanTest
    {
        [Test()]
        public void TestDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "Legacy", "InsulaPrimalis");

            StartGame();

            RemoveVillainTriggers();
            RemoveVillainCards();

            DecisionSelectTarget = baron.CharacterCard;
            AssertDamageType(DamageType.Projectile);
            AssertDamageSource(alexandria.CharacterCard);
            QuickHPStorage(baron.CharacterCard);
            PlayCard("PureStrength");
            QuickHPCheck(-4);

            DecisionSelectTarget = legacy.CharacterCard;
            AssertDamageType(DamageType.Projectile);
            AssertDamageSource(alexandria.CharacterCard);
            QuickHPStorage(legacy.CharacterCard);
            PlayCard("PureStrength");
            QuickHPCheck(-4);
        }
    }
}
