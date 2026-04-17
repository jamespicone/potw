using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Armsmaster
{
    [TestFixture()]
    public class StasisEffectorTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();
        }

        [Test()]
        public void TestIsEquipmentModule()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            var card = GetCard("StasisEffector");
            Assert.That(card.DoKeywordsContain("equipment"), Is.True);
            Assert.That(card.DoKeywordsContain("module"), Is.True);
        }

        [Test()]
        public void TestPrimaryPreventsVillainPlay()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            // Ensure Armsmaster has at least 2 cards in hand to discard
            PutInHand(GetCard("DualWielding", 0));
            PutInHand(GetCard("DualWielding", 1));

            var halberd = PlayCard("SurveyHalberd");
            DecisionSelectWord = "Primary";
            var stasis = PlayCard("StasisEffector");

            DecisionActivateAbilities = new Card[] { stasis };
            UsePower(halberd);

            // The next villain card play should be prevented
            var villainTopCard = baron.TurnTaker.Deck.TopCard;
            PlayCard(villainTopCard);
            // Card play was cancelled - card should not be in play
            AssertNotInPlay(villainTopCard);
        }

        [Test()]
        public void TestPrimaryVillainImmune()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            // Ensure Armsmaster has at least 2 cards in hand
            PutInHand(GetCard("DualWielding", 0));
            PutInHand(GetCard("DualWielding", 1));

            var halberd = PlayCard("SurveyHalberd");
            DecisionSelectWord = "Primary";
            var stasis = PlayCard("StasisEffector");

            DecisionActivateAbilities = new Card[] { stasis };
            UsePower(halberd);

            // Villain character cards should be immune to damage until start of next turn
            QuickHPStorage(baron);
            DealDamage(armsmaster, baron, 5, DamageType.Melee);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestPrimaryImmunityExpires()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            PutInHand(GetCard("DualWielding", 0));
            PutInHand(GetCard("DualWielding", 1));

            var halberd = PlayCard("SurveyHalberd");
            DecisionSelectWord = "Primary";
            var stasis = PlayCard("StasisEffector");

            DecisionActivateAbilities = new Card[] { stasis };
            UsePower(halberd);

            // Advance to start of Armsmaster's next turn to expire immunity
            GoToStartOfTurn(armsmaster);

            // Now damage should go through
            QuickHPStorage(baron);
            DealDamage(armsmaster, baron, 5, DamageType.Melee);
            QuickHPCheck(-5);
        }

        [Test()]
        public void TestSecondaryDestroysLowHPTarget()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            // Use MDP as the low-HP target
            var mdp = GetMobileDefensePlatform().Card;
            SetHitPoints(mdp, 4);

            var halberd = PlayCard("SurveyHalberd");
            DecisionSelectWord = "Secondary";
            var stasis = PlayCard("StasisEffector");

            DecisionSelectCard = mdp;
            DecisionActivateAbilities = new Card[] { stasis };

            UsePower(halberd);

            AssertInTrash(mdp);
        }
    }
}
