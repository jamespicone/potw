using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.TheMerchants
{
    [TestFixture()]
    public class HelicopterTests : ParahumanTest
    {
        [Test()]
        public void TestImmuneToMelee()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "InsulaPrimalis");

            StartGame();

            PlayCard("Helicopter");

            QuickHPStorage(merchants.CharacterCard);
            DealDamage(tempest, merchants.CharacterCard, 5, DamageType.Melee);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestPlaysThugs()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            PlayCard("Helicopter");

            GoToPlayCardPhase(merchants);
            AssertNumberOfCardsInPlay(c => c.DoKeywordsContain("thug"), 0);

            GoToEndOfTurn(merchants);
            AssertNumberOfCardsInPlay(c => c.DoKeywordsContain("thug"), 1);

            GoToPlayCardPhase(merchants);
            AssertNumberOfCardsInPlay(c => c.DoKeywordsContain("thug"), 1);

            GoToEndOfTurn(merchants);
            AssertNumberOfCardsInPlay(c => c.DoKeywordsContain("thug"), 2);
        }
    }
}
