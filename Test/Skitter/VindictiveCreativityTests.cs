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
    public class VindictiveCreativityTests : ParahumanTest
    {
        [Test()]
        public void IncreaseSwarmDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            PlayCard("DeadlySpiders");
            PlayCard("VindictiveCreativity");

            QuickHPStorage(baron, skitter);
            DecisionSelectTargets = new Card[] { baron.CharacterCard };

            GoToEndOfTurn(skitter);
            QuickHPCheck(-2, 0);
        }

        [Test()]
        public void DontIncreaseSkitterDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            PlayCard("VindictiveCreativity");

            QuickHPStorage(baron, skitter);
            DealDamage(skitter, baron, 1, DamageType.Fire);
            QuickHPCheck(-1, 0);
        }

        [Test()]
        public void ChangeDamageType()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            PlayCard("DeadlySpiders");
            PlayCard("VindictiveCreativity");
            PlayCard("ElementalRedistributor");

            QuickHPStorage(baron, skitter);
            DecisionSelectTargets = new Card[] { baron.CharacterCard };
            DecisionSelectDamageType = DamageType.Fire;

            GoToEndOfTurn(skitter);
            QuickHPCheck(0, -1);
        }
    }
}
