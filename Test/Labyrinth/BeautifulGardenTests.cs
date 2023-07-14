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
    public class BeautifulGardenTests : BaseTest
    {
        protected HeroTurnTakerController labyrinth { get { return FindHero("Labyrinth"); } }
        protected HeroTurnTakerController tattletale { get { return FindHero("Tattletale"); } }

        [Test()]
        public void PreventsDamageToHeroes()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Jp.ParahumansOfTheWormverse.Tattletale", "InsulaPrimalis");

            RemoveVillainTriggers();
            StartGame();            

            PlayCard("ObsidianField");
            PlayCard("BeautifulGarden");

            var raptors = PlayCard("VelociraptorPack");

            QuickHPStorage(tattletale);
            DealDamage(raptors, tattletale.CharacterCard, 5, DamageType.Infernal);
            QuickHPCheck(0);
            DealDamage(raptors, tattletale.CharacterCard, 5, DamageType.Infernal);
            QuickHPCheck(-5);
        }

        [Test()]
        public void DoesntProtectLabyrinth()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Jp.ParahumansOfTheWormverse.Tattletale", "InsulaPrimalis");

            RemoveVillainTriggers();
            StartGame();

            PlayCard("ObsidianField");
            PlayCard("BeautifulGarden");

            var raptors = PlayCard("VelociraptorPack");

            QuickHPStorage(labyrinth);
            DealDamage(raptors, labyrinth.CharacterCard, 5, DamageType.Infernal);
            QuickHPCheck(-5);
            DealDamage(raptors, labyrinth.CharacterCard, 5, DamageType.Infernal);
            QuickHPCheck(-5);
        }

        [Test()]
        public void LabyrinthDoesntPreventReduction()
        {
            SetupGameController("BaronBlade", "Legacy", "Jp.ParahumansOfTheWormverse.Labyrinth", "Jp.ParahumansOfTheWormverse.Tattletale", "InsulaPrimalis");

            RemoveVillainTriggers();
            StartGame();

            PlayCard("ObsidianField");
            PlayCard("BeautifulGarden");

            var raptors = PlayCard("VelociraptorPack");

            QuickHPStorage(labyrinth, tattletale, legacy);
            DealDamage(raptors, labyrinth.CharacterCard, 5, DamageType.Infernal);
            QuickHPCheck(-5, 0, 0);
            DealDamage(raptors, tattletale.CharacterCard, 5, DamageType.Infernal);
            QuickHPCheck(0, 0, 0);
            DealDamage(raptors, legacy.CharacterCard, 5, DamageType.Infernal);
            QuickHPCheck(0, 0, -5);
        }

        [Test()]
        public void GardenPreventsDestruction()
        {
            SetupGameController("BaronBlade", "TheWraith", "Jp.ParahumansOfTheWormverse.Labyrinth", "Jp.ParahumansOfTheWormverse.Tattletale", "OmnitronIV");

            RemoveVillainTriggers();
            StartGame();
            
            var bombs = PlayCard("SmokeBombs");

            PlayCard("OverchargedSystems");
            PlayCard("BeautifulGarden");

            PlayCard("MechaniAccumulator");

            GoToEndOfTurn(env);

            AssertInPlayArea(wraith, bombs);
        }
    }
}
