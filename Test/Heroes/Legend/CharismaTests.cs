using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Legend
{
    [TestFixture()]
    public class CharismaTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
        }

        [Test()]
        public void TestIsOneShot()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "InsulaPrimalis");
            StartGame();

            var card = GetCard("Charisma");
            Assert.That(card.IsOneShot, Is.True);
        }

        [Test()]
        public void TestOtherPlayerRegainsHP()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "Bunker", "InsulaPrimalis");
            StartGame();

            DealDamage(baron, bunker, 5, DamageType.Melee);

            // Select "Regain 2 HP" for Bunker (function index 0)
            DecisionSelectFunction = 0;

            QuickHPStorage(bunker);
            PlayCard("Charisma");
            QuickHPCheck(2);
        }

        [Test()]
        public void TestOtherPlayerPlaysCard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "Bunker", "InsulaPrimalis");
            StartGame();

            var bunkerCard = PutInHand(bunker, "FlakCannon");

            // Select "Play a card" for Bunker (function index 1)
            DecisionSelectFunction = 1;
            DecisionSelectCard = bunkerCard;

            PlayCard("Charisma");

            AssertIsInPlay(bunkerCard);
        }

        [Test()]
        public void TestDoesNotAffectLegend()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "Bunker", "InsulaPrimalis");
            StartGame();

            DealDamage(baron, legend, 5, DamageType.Melee);
            DealDamage(baron, bunker, 5, DamageType.Melee);

            DecisionSelectFunction = 0;

            QuickHPStorage(legend, bunker);
            PlayCard("Charisma");
            // Legend not affected, Bunker regains 2 HP
            QuickHPCheck(0, 2);
        }

        [Test()]
        public void TestMultipleOtherPlayers()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Legend", "Bunker", "Haka", "InsulaPrimalis");
            StartGame();

            DealDamage(baron, bunker, 5, DamageType.Melee);
            DealDamage(baron, FindHero("Haka"), 5, DamageType.Melee);

            // Both select "Regain 2 HP"
            DecisionSelectFunction = 0;

            QuickHPStorage(bunker.CharacterCard, FindHero("Haka").CharacterCard);
            PlayCard("Charisma");
            QuickHPCheck(2, 2);
        }
    }
}
