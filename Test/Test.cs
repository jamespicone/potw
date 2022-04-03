using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;
using Jp.ParahumansOfTheWormverse.Bitch;

namespace Jp.ParahumansOfTheWormverse.UnitTest
{
    [TestFixture()]
    public class BitchTest : BaseTest
    {
        protected HeroTurnTakerController bitch { get { return FindHero("Bitch"); } }

        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "Megalopolis"); //  "Jp.ParahumansOfTheWormverse.Bitch"

            Assert.AreEqual(3, GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(bitch);
            Assert.IsInstanceOf(typeof(HeroTurnTakerController), bitch);
            Assert.IsInstanceOf(typeof(BitchCharacterCardController), bitch.CharacterCardController);

            Assert.IsNotNull(env);

            Assert.AreEqual(28, bitch.CharacterCard.HitPoints);

            GoToStartOfTurn(bitch);
            PlayCard(bitch, "Milk");
            var milk = FindCardInPlay("Milk");
            Assert.IsNotNull(milk);
            Assert.AreEqual(6, milk.HitPoints);

            GoToStartOfTurn(bitch);
            Assert.AreEqual(5, milk.HitPoints);

            PlayCard(bitch, "VeterinaryCare");
            Assert.AreEqual(6, milk.HitPoints);
        }

        //[Test()]
        //public void TestHeroicInfinitorVsMirrors()
        //{
        //    SetupGameController(
        //        new[] { "Infinitor", "Legacy", "Tempest", "MMFFCC" },
        //        promoIdentifiers: new Dictionary<string, string>{
        //            { "Infinitor", "HeroicInfinitorCharacter" }
        //        }
        //    );

        //    StartGame();

        //    PlayCard("MazeOfMirrors");
        //    PlayCard("LeadFromTheFront");

        //    AssertNoDecision();

        //    // MoM redirects the first damage done to the source
        //    DealDamage(infinitor, legacy, 1, DamageType.Radiant);

        //    // "Whenever a hero target would be dealt damage by a ~villain~ hero card,
        //    // you may redirect that damage to legacy.
        //    // 
        //    // Shouldn't get the option to redirect; a villain card is dealing damage to a hero target.
        //    DealDamage(infinitor, tempest, 1, DamageType.Radiant);
        //}

        //[Test()]
        //public void TestHeroicInfinitorVsGroundPound()
        //{
        //    SetupGameController(
        //        new[] { "Infinitor", "Haka", "Megalopolis" },
        //        promoIdentifiers: new Dictionary<string, string>{
        //            { "Infinitor", "HeroicInfinitorCharacter" }
        //        }
        //    );

        //    StartGame();

        //    DecisionYesNo = true;
        //    DecisionSelectCards = haka.HeroTurnTaker.Hand.Cards;
        //    PlayCard("GroundPound");

        //    QuickHPStorage(haka);

        //    // Ground-Pound prevents non-hero cards from dealing damage.
        //    // Heroic Infinitor is not a hero card; he shouldn't deal damage.
        //    DealDamage(infinitor, haka, 1, DamageType.Radiant);

        //    QuickHPCheck(0);
        //}
    }
}
