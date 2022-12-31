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
    public class ImprovisationalIngenuityTests : BaseTest
    {
        protected HeroTurnTakerController skitter { get { return FindHero("Skitter"); } }
        protected HeroTurnTakerController tattletale { get { return FindHero("Tattletale"); } }

        [Test()]
        public void TestPower()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            var stacked = StackDeck("SabotageSwarm", "SpidersilkArmour", "SweepTheArea");
            var ingenuity = PlayCard("ImprovisationalIngenuity");

            DecisionSelectCard = stacked.FirstOrDefault();
            AssertNextDecisionChoices(stacked);

            QuickHandStorage(skitter);
            UsePower(ingenuity, 0);
            QuickHandCheck(1);

            AssertInHand(skitter, stacked.FirstOrDefault());
            AssertInDeck(skitter, stacked.Skip(1));
        }

        [Test()]
        public void TestSkitterDamagePrevented()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();

            MoveAllCardsFromHandToDeck(skitter);

            PlayCard("ImprovisationalIngenuity");

            AssertNextDecisionSelectionType(SelectionType.UsePower);

            DecisionSelectPower = skitter.CharacterCard;
            DealDamage(skitter, baron, 1, DamageType.Infernal);

            var pool = skitter.CharacterCard.FindBugPool();
            AssertTokenPoolCount(pool, 1);
        }

        [Test()]
        public void TestSkitterDamageReducedToZero()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            MoveAllCardsFromHandToDeck(skitter);

            PlayCard("LivingForceField");
            PlayCard("ImprovisationalIngenuity");

            AssertNextDecisionSelectionType(SelectionType.UsePower);

            DecisionSelectPower = skitter.CharacterCard;
            DealDamage(skitter, baron, 1, DamageType.Infernal);

            var pool = skitter.CharacterCard.FindBugPool();
            AssertTokenPoolCount(pool, 1);
        }

        [Test()]
        public void TestSwarmDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();

            MoveAllCardsFromHandToDeck(skitter);

            PlayCard("ImprovisationalIngenuity");
            PlayCard("DeadlySpiders");

            DecisionSelectCards = new Card[] { baron.CharacterCard, null };

            GoToEndOfTurn(skitter);

            var pool = skitter.CharacterCard.FindBugPool();
            AssertTokenPoolCount(pool, 1);
        }
    }
}
