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
    public class CruelCityTests : BaseTest
    {
        protected HeroTurnTakerController labyrinth { get { return FindHero("Labyrinth"); } }
        protected HeroTurnTakerController tattletale { get { return FindHero("Tattletale"); } }

        [Test()]
        public void TestDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Jp.ParahumansOfTheWormverse.Tattletale", "InsulaPrimalis");

            StartGame();
            RemoveMobileDefensePlatform();

            DecisionSelectCard = baron.CharacterCard;
            DecisionSelectDamageType = DamageType.Infernal;

            QuickHPStorage(baron);
            PlayCard("CruelCity");
            QuickHPCheck(-4);
        }

        [Test()]
        public void TestDestroyingShapingDestroysOngoing()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Jp.ParahumansOfTheWormverse.Tattletale", "InsulaPrimalis");

            StartGame();

            var forcefield = PlayCard("LivingForceField");

            PlayCard("ObsidianField");
            var garden = PlayCard("BeautifulGarden");

            StackDeck("VelociraptorPack");

            AssertIsInPlay(garden);

            DecisionSelectCards = new Card[] { baron.CharacterCard, garden, forcefield, null };
            AssertIsInPlay(forcefield);
            PlayCard("CruelCity");
            AssertInTrash(forcefield);
            AssertInTrash(garden);
        }

        [Test()]
        public void TestPlaysEnvironment()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Jp.ParahumansOfTheWormverse.Tattletale", "InsulaPrimalis");

            StartGame();

            var raptors = StackDeck("VelociraptorPack");

            PlayCard("CruelCity");
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

            DecisionDoNotSelectCard = SelectionType.DestroyCard;
            PlayCard("CruelCity");

            AssertIsInPlay(garden);
        }
    }
}
