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
    public class ApocryphaTests : EchidnaBaseTest
    {
        protected HeroTurnTakerController alexandria { get { return FindHero("Alexandria"); } }
        protected HeroTurnTakerController bitch { get { return FindHero("Bitch"); } }

        protected HeroTurnTakerController legend { get { return FindHero("Legend"); } }

        [Test()]
        public void TestDamageReduction()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "InsulaPrimalis"
            );

            StartGame();
            ReturnAllTwisted();

            PlayCard("ApocryphaTwisted");

            // Immune to damage
            QuickHPStorage(echidna.CharacterCard);
            DealDamage(alexandria.CharacterCard, echidna.CharacterCard, 2, DamageType.Infernal);
            QuickHPCheck(0);
            DealDamage(alexandria.CharacterCard, echidna.CharacterCard, 2, DamageType.Infernal);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestDamage()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "Jp.ParahumansOfTheWormverse.Legend",
                "InsulaPrimalis"
            );

            RemoveAllTwisted();

            StartGame();
            GoToPlayCardPhase(echidna);
            
            DestroyNonCharacterVillainCards();

            var apocrypha = PlayCard("ApocryphaTwisted");
            QuickHPStorage(alexandria, bitch, legend);

            EnterNextTurnPhase();

            // Echidna hits bitch and legend; apocrypha hits alexandria
            QuickHPCheck(-2, -2, -2);
        }
    }
}
