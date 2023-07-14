using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Jp.ParahumansOfTheWormverse.Labyrinth;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Labyrinth
{
    [TestFixture()]
    public class DefensiveButtressTests : BaseTest
    {
        protected HeroTurnTakerController labyrinth { get { return FindHero("Labyrinth"); } }
        protected HeroTurnTakerController tattletale { get { return FindHero("Tattletale"); } }

        [Test()]
        public void TestStatusEffect()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Jp.ParahumansOfTheWormverse.Tattletale", "InsulaPrimalis");

            StartGame();
            StackDeck("VelociraptorPack");

            PlayCard("DefensiveButtress");

            AssertNumberOfStatusEffectsInPlay(1);

            QuickHPStorage(labyrinth, tattletale);
            DealDamage(labyrinth, labyrinth, 1, DamageType.Infernal);
            DealDamage(tattletale, tattletale, 2, DamageType.Infernal);
            QuickHPCheck(0, -1);
        }

        [Test()]
        public void TestPlaysEnvironment()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Jp.ParahumansOfTheWormverse.Tattletale", "InsulaPrimalis");

            StartGame();

            var raptors = StackDeck("VelociraptorPack");

            PlayCard("DefensiveButtress");
            AssertIsInPlay(raptors);
        }

        [Test()]
        public void TestDestroyingShapingOptional()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Jp.ParahumansOfTheWormverse.Tattletale", "InsulaPrimalis");

            StartGame();

            PlayCard("ObsidianField");
            var garden = PlayCard("BeautifulGarden");

            AssertIsInPlay(garden);

            // Stack the deck so an unlucky play doesn't stall out the test
            StackDeck("RiverOfLava");

            DecisionDoNotSelectCard = SelectionType.DestroyCard;
            PlayCard("DefensiveButtress");

            AssertIsInPlay(garden);
        }

        [Test()]
        public void TestDestroyingShapingStashesEnv()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Jp.ParahumansOfTheWormverse.Tattletale", "InsulaPrimalis");

            StartGame();

            PlayCard("ObsidianField");
            var garden = PlayCard("BeautifulGarden");
            var raptors = PlayCard("VelociraptorPack");

            AssertIsInPlay(garden);

            // Stack the deck so an unlucky play doesn't stall out the test
            StackDeck("RiverOfLava");

            DecisionSelectCards = new Card[] { garden, raptors, null };
            PlayCard("DefensiveButtress");

            AssertUnderCard(labyrinth.CharacterCard, raptors);
            AssertInTrash(garden);
        }
    }
}
