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
    public class ConvictionTests : EchidnaBaseTest
    {
        protected HeroTurnTakerController alexandria { get { return FindHero("Alexandria"); } }
        protected HeroTurnTakerController bitch { get { return FindHero("Bitch"); } }

        protected HeroTurnTakerController legend { get { return FindHero("Legend"); } }

        [Test()]
        public void TestImmuneToMelee()
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

            var conviction = PlayCard("ConvictionTwisted");
            QuickHPStorage(conviction);
            DealDamage(alexandria, conviction, 3, DamageType.Melee);
            QuickHPCheck(0);
            DealDamage(alexandria, conviction, 3, DamageType.Projectile);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestDealsDamage()
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

            StackDeck("ChimaericalNightmare");

            GoToEndOfTurn(echidna);
            PlayCard("ConvictionTwisted");

            QuickHPStorage(alexandria, bitch);
            EnterNextTurnPhase();
            QuickHPCheck(-1, 0);
            GoToStartOfTurn(bitch);
            QuickHPCheck(0, -1);
        }
    }
}
