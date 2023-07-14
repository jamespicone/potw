using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Jp.ParahumansOfTheWormverse.Skitter;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Skitter
{
    [TestFixture()]
    public class SwarmOfFliesTests : ParahumanTest
    {
        [Test()]
        public void ReducesMeleeAndProjectile()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            var flies = PlayCard("SwarmOfFlies");

            QuickHPStorage(skitter);
            DealDamage(baron, skitter, 1, DamageType.Melee);
            QuickHPCheck(0);

            DealDamage(baron, skitter, 1, DamageType.Projectile);
            QuickHPCheck(0);

            QuickHPStorage(flies);
            DealDamage(baron, flies, 1, DamageType.Melee);
            QuickHPCheck(0);
            AssertIsInPlay(flies);

            DealDamage(baron, flies, 1, DamageType.Projectile);
            QuickHPCheck(0);
            AssertIsInPlay(flies);
        }

        [Test()]
        public void Adds2Bugs()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            var pool = skitter.CharacterCard.FindBugPool();
            AssertTokenPoolCount(pool, 0);

            PlayCard("SwarmOfFlies");

            AssertTokenPoolCount(pool, 2);
        }

        [Test()]
        public void LoseBugsOnDestroy()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            var pool = skitter.CharacterCard.FindBugPool();
            AssertTokenPoolCount(pool, 0);

            var flies = PlayCard("SwarmOfFlies");

            AssertTokenPoolCount(pool, 2);

            DestroyCard(flies);

            AssertTokenPoolCount(pool, 1);
        }
    }
}
