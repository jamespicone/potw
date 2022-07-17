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
    public class AbhorrorTests : EchidnaBaseTest
    {
        protected HeroTurnTakerController alexandria { get { return FindHero("Alexandria"); } }
        protected HeroTurnTakerController bitch { get { return FindHero("Bitch"); } }

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

            var abhorror = PlayCard("AbhorrorTwisted");

            QuickHPStorage(abhorror);
            DealDamage(alexandria.CharacterCard, abhorror, 2, DamageType.Infernal);
            QuickHPCheck(-1);

            QuickHPStorage(echidna.CharacterCard);
            DealDamage(alexandria.CharacterCard, echidna.CharacterCard, 2, DamageType.Infernal);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestTwistedBounce()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "InsulaPrimalis"
            );

            StartGame();
            GoToPlayCardPhase(echidna);

            ReturnAllTwisted();
            DestroyNonCharacterVillainCards();

            var abhorror = PlayCard("AbhorrorTwisted");
            var hubris = PlayCard("HubrisTwisted");
            var pandemic = PlayCard("PandemicTwisted");

            var rout = StackDeck("RoutTwisted");
            var propaganda = StackDeck("PropagandaTwisted");

            EnterNextTurnPhase();

            // Play propaganda, then shuffle pandemic back in.
            AssertAtLocation(
                new Card[] { abhorror, hubris, propaganda },
                echidna.TurnTaker.PlayArea
            );

            AssertAtLocation(
                new Card[] { pandemic, rout },
                echidna.TurnTaker.FindSubDeck("TwistedDeck")
            );
        }
    }
}
