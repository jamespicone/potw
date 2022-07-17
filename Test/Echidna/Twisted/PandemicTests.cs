using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Echidna
{
    [TestFixture()]
    public class PandemicTests : EchidnaBaseTest
    {
        protected HeroTurnTakerController alexandria { get { return FindHero("Alexandria"); } }
        protected HeroTurnTakerController bitch { get { return FindHero("Bitch"); } }

        protected HeroTurnTakerController legend { get { return FindHero("Legend"); } }

        [Test()]
        public void TestPreventsDeath()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "InsulaPrimalis"
            );

            StartGame();
            DestroyNonCharacterVillainCards();
            ReturnAllTwisted();

            PlayCard("PandemicTwisted");
            var ariadne = PlayCard("AriadneTwisted");

            QuickHPStorage(ariadne);
            DealDamage(alexandria, ariadne, 5, DamageType.Radiant);
            QuickHPCheck(-5);
            DealDamage(alexandria, ariadne, 10, DamageType.Radiant);
            QuickHPCheck(-4);
            AssertIsInPlay(ariadne);

            DealDamage(alexandria, ariadne, 1, DamageType.Radiant);
            AssertNotInPlay(ariadne);
        }

        [Test()]
        public void TestHeal()
        {
            SetupGameController(
                            "Jp.ParahumansOfTheWormverse.Echidna",
                            "Jp.ParahumansOfTheWormverse.Alexandria",
                            "Jp.ParahumansOfTheWormverse.Bitch",
                            "InsulaPrimalis"
                        );

            StartGame();
            DestroyNonCharacterVillainCards();
            ReturnAllTwisted();

            var pandemic = PlayCard("PandemicTwisted");
            StackDeck("ChimaericalNightmare");

            GoToPlayCardPhase(echidna);

            DealDamage(echidna, pandemic, 5, DamageType.Infernal);
            DealDamage(echidna, echidna.CharacterCard, 5, DamageType.Infernal);

            QuickHPStorage(echidna.CharacterCard, pandemic);
            EnterNextTurnPhase();
            QuickHPCheck(2, 2);
        }
    }
}
