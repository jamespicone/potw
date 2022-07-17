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
    public class SlipperyTests : EchidnaBaseTest
    {
        protected HeroTurnTakerController alexandria { get { return FindHero("Alexandria"); } }
        protected HeroTurnTakerController bitch { get { return FindHero("Bitch"); } }

        [Test()]
        public void TestWorks()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "InsulaPrimalis"
            );

            StartGame();
            ReturnAllTwisted();
            DestroyNonCharacterVillainCards();

            DecisionSelectTurnTaker = alexandria.TurnTaker;

            QuickHandStorage(alexandria, bitch);
            PlayCard("Slippery");
            QuickHandCheck(-4, 0);

            QuickHPStorage(echidna.CharacterCard);
            DealDamage(echidna.CharacterCard, echidna.CharacterCard, 1, DamageType.Radiant);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestSkip()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "InsulaPrimalis"
            );

            StartGame();
            GoToStartOfTurn(alexandria);
            ReturnAllTwisted();
            DestroyNonCharacterVillainCards();

            DecisionDoNotSelectTurnTaker = true;

            QuickHandStorage(alexandria, bitch);
            PlayCard("Slippery");
            QuickHandCheck(0, 0);

            QuickHPStorage(echidna.CharacterCard);
            DealDamage(echidna.CharacterCard, echidna.CharacterCard, 1, DamageType.Radiant);
            QuickHPCheck(0);

            GoToEndOfTurn(FindEnvironment());

            QuickHPStorage(echidna.CharacterCard);
            DealDamage(echidna.CharacterCard, echidna.CharacterCard, 1, DamageType.Radiant);
            QuickHPCheck(0);

            EnterNextTurnPhase();

            QuickHPStorage(echidna.CharacterCard);
            DealDamage(echidna.CharacterCard, echidna.CharacterCard, 1, DamageType.Radiant);
            QuickHPCheck(-1);
        }
    }
}
