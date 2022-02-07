using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
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
    }
}
