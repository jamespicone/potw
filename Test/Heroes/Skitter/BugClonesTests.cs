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
    public class BugClonesTests : ParahumanTest
    {
        [Test()]
        public void DamagePreventionIsOptional()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();

            DecisionYesNo = false;

            QuickHPStorage(skitter);
            PlayCard("BugClones");
            DealDamage(skitter.CharacterCard, skitter.CharacterCard, 1, DamageType.Psychic);
            QuickHPCheck(-1);
        }

        [Test()]
        public void DestroyCardIfOutOfTokens()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();

            DecisionYesNo = true;

            QuickHPStorage(skitter);
            var clones = PlayCard("BugClones");
            DealDamage(skitter.CharacterCard, skitter.CharacterCard, 1, DamageType.Psychic);
            QuickHPCheck(0);

            AssertNotInPlay(clones);
        }

        [Test()]
        public void DestroyCardIfOneToken()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();

            DecisionYesNo = true;

            QuickHPStorage(skitter);
            var clones = PlayCard("BugClones");
            var pool = clones.FindBugPool();
            pool.AddTokens(1);
            DealDamage(skitter.CharacterCard, skitter.CharacterCard, 1, DamageType.Psychic);
            QuickHPCheck(0);

            AssertNotInPlay(clones);
        }

        [Test()]
        public void CanTakeTokensInsteadOfDestroy()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();

            DecisionYesNo = true;
            DecisionSelectFunction = 0;

            QuickHPStorage(skitter);
            var clones = PlayCard("BugClones");
            var pool = clones.FindBugPool();
            pool.AddTokens(2);
            DealDamage(skitter.CharacterCard, skitter.CharacterCard, 1, DamageType.Psychic);
            QuickHPCheck(0);

            AssertIsInPlay(clones);
            AssertTokenPoolCount(pool, 0);
        }

        [Test()]
        public void CanStillDestroyInsteadOfTake()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();

            DecisionYesNo = true;
            DecisionSelectFunction = 1;

            QuickHPStorage(skitter);
            var clones = PlayCard("BugClones");
            var pool = clones.FindBugPool();
            pool.AddTokens(2);
            DealDamage(skitter.CharacterCard, skitter.CharacterCard, 1, DamageType.Psychic);
            QuickHPCheck(0);

            AssertNotInPlay(clones);
        }

        [Test()]
        public void DontPreventZeroDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();

            AssertNoDecision(SelectionType.PreventDamage);

            QuickHPStorage(skitter);
            PlayCard("BugClones");
            PlayCard("SpidersilkArmour");
            DealDamage(skitter.CharacterCard, skitter.CharacterCard, 1, DamageType.Psychic);
            QuickHPCheck(0);
        }
    }
}
