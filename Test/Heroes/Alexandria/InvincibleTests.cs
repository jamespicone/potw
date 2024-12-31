using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Jp.ParahumansOfTheWormverse.Tattletale;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Alexandria
{
    [TestFixture()]
    public class InvincibleTests : ParahumanTest
    {
        [Test()]
        public void TestStatusEffect()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "Megalopolis");

            StartGame();

            PlayCard("Invincible");

            DecisionYesNo = true;

            var cardToDiscard = alexandria.HeroTurnTaker.Hand.Cards.First();
            DecisionDiscardCard = cardToDiscard;

            QuickHPStorage(alexandria.CharacterCard);
            QuickHandStorage(alexandria);

            DealDamage(baron.CharacterCard, alexandria.CharacterCard, 2, DamageType.Melee);

            QuickHPCheck(0);
            QuickHandCheck(-1);

            AssertAtLocation(cardToDiscard, alexandria.TurnTaker.Trash);
        }

        [Test()]
        public void TestDeclineToDiscard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "Megalopolis");

            StartGame();

            PlayCard("Invincible");

            DecisionYesNo = false;

            QuickHPStorage(alexandria.CharacterCard);
            QuickHandStorage(alexandria);

            DealDamage(baron.CharacterCard, alexandria.CharacterCard, 2, DamageType.Melee);

            QuickHPCheck(-1);
            QuickHandCheck(0);
        }

        [Test()]
        public void TestCardDestroyed()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "Megalopolis");

            StartGame();

            var invincible = PlayCard("Invincible");
            DestroyCard(invincible);

            DecisionYesNo = true;

            var cardToDiscard = alexandria.HeroTurnTaker.Hand.Cards.First();
            DecisionDiscardCard = cardToDiscard;

            QuickHPStorage(alexandria.CharacterCard);
            QuickHandStorage(alexandria);

            DealDamage(baron.CharacterCard, alexandria.CharacterCard, 2, DamageType.Melee);

            QuickHPCheck(0);
            QuickHandCheck(-1);

            AssertAtLocation(cardToDiscard, alexandria.TurnTaker.Trash);
        }

        [Test()]
        public void TestWithRedirectAwayRedirectFirst()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            StartGame();

            PlayCard("Invincible");
            var wittyDeflection = PlayCard("WittyDeflection");

            DecisionSelectCard = alexandria.CharacterCard;

            UsePower(wittyDeflection);

            ResetDecisions();

            DecisionAmbiguousCard = wittyDeflection;
            DecisionRedirectTarget = tattletale.CharacterCard;

            QuickHPStorage(alexandria.CharacterCard, tattletale.CharacterCard);
            QuickHandStorage(alexandria);

            DealDamage(baron.CharacterCard, alexandria.CharacterCard, 2, DamageType.Melee);

            QuickHPCheck(0, -2);
            QuickHandCheck(0);
        }

        [Test()]
        public void TestWithRedirectAwayRedirectFirstThenSkip()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            StartGame();

            PlayCard("Invincible");
            var wittyDeflection = PlayCard("WittyDeflection");

            DecisionSelectCard = alexandria.CharacterCard;

            UsePower(wittyDeflection);

            ResetDecisions();

            DecisionAmbiguousCard = wittyDeflection;
            DecisionRedirectTargets = new Card[] { null };

            DecisionYesNo = true;
            var cardToDiscard = alexandria.HeroTurnTaker.Hand.Cards.First();
            DecisionDiscardCard = cardToDiscard;

            QuickHPStorage(alexandria.CharacterCard, tattletale.CharacterCard);
            QuickHandStorage(alexandria);

            DealDamage(baron.CharacterCard, alexandria.CharacterCard, 2, DamageType.Melee);

            QuickHPCheck(0, 0);
            QuickHandCheck(-1);

            AssertAtLocation(cardToDiscard, alexandria.TurnTaker.Trash);
        }

        [Test()]
        public void TestWithRedirectAwayInvincibleFirstDiscard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            StartGame();

            var invincible = PlayCard("Invincible");
            var wittyDeflection = PlayCard("WittyDeflection");

            DecisionSelectCard = alexandria.CharacterCard;

            UsePower(wittyDeflection);

            ResetDecisions();

            DecisionAmbiguousCard = invincible;

            // expect this not to be used; here so that we redirect to tattletale if invincible is broken.
            DecisionRedirectTarget = tattletale.CharacterCard; 

            DecisionYesNo = true;
            var cardToDiscard = alexandria.HeroTurnTaker.Hand.Cards.First();
            DecisionDiscardCard = cardToDiscard;

            QuickHPStorage(alexandria.CharacterCard, tattletale.CharacterCard);
            QuickHandStorage(alexandria);

            DealDamage(baron.CharacterCard, alexandria.CharacterCard, 2, DamageType.Melee);

            QuickHPCheck(0, 0);
            QuickHandCheck(-1);

            AssertAtLocation(cardToDiscard, alexandria.TurnTaker.Trash);
        }

        [Test()]
        public void TestWithRedirectAwayInvincibleFirstDontDiscard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            StartGame();

            var invincible = PlayCard("Invincible");
            var wittyDeflection = PlayCard("WittyDeflection");

            DecisionSelectCard = alexandria.CharacterCard;

            UsePower(wittyDeflection);

            ResetDecisions();

            DecisionAmbiguousCard = invincible;

            DecisionRedirectTarget = tattletale.CharacterCard;

            DecisionYesNo = false;

            QuickHPStorage(alexandria.CharacterCard, tattletale.CharacterCard);
            QuickHandStorage(alexandria);

            DealDamage(baron.CharacterCard, alexandria.CharacterCard, 2, DamageType.Melee);

            QuickHPCheck(0, -2);
            QuickHandCheck(0);
        }

        [Test()]
        public void TestWithRedirectAwayInvincibleFirstDontDiscardThenSkip()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            StartGame();

            var invincible = PlayCard("Invincible");
            var wittyDeflection = PlayCard("WittyDeflection");

            DecisionSelectCard = alexandria.CharacterCard;

            UsePower(wittyDeflection);

            ResetDecisions();

            DecisionAmbiguousCard = invincible;

            DecisionRedirectTargets = new Card[] { null };

            // Expect only the first one to get used; the second value is there
            // so that if we have a bug that makes invincible trigger twice it's visible.
            DecisionsYesNo = new bool[] { false, true };

            QuickHPStorage(alexandria.CharacterCard, tattletale.CharacterCard);
            QuickHandStorage(alexandria);

            DealDamage(baron.CharacterCard, alexandria.CharacterCard, 2, DamageType.Melee);

            QuickHPCheck(-1, 0);
            QuickHandCheck(0);

            // TODO: This triggers invincible twice; fail test in that case.
        }

        [Test()]
        public void TestWithRedirectToDiscard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            StartGame();

            PlayCard("Invincible");
            var wittyDeflection = PlayCard("WittyDeflection");

            DecisionYesNo = true;

            DecisionSelectCard = tattletale.CharacterCard;
            DecisionRedirectTarget = alexandria.CharacterCard;

            UsePower(wittyDeflection);

            var cardToDiscard = alexandria.HeroTurnTaker.Hand.Cards.First();
            DecisionDiscardCard = cardToDiscard;

            QuickHPStorage(alexandria.CharacterCard, tattletale.CharacterCard);
            QuickHandStorage(alexandria);

            DealDamage(baron.CharacterCard, tattletale.CharacterCard, 2, DamageType.Melee);

            QuickHPCheck(0, 0);
            QuickHandCheck(-1);

            AssertAtLocation(cardToDiscard, alexandria.TurnTaker.Trash);
        }

        [Test()]
        public void TestWithRedirectToDontDiscard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            StartGame();

            PlayCard("Invincible");
            var wittyDeflection = PlayCard("WittyDeflection");

            DecisionYesNo = false;

            DecisionSelectCard = tattletale.CharacterCard;
            DecisionRedirectTarget = alexandria.CharacterCard;

            UsePower(wittyDeflection);

            QuickHPStorage(alexandria.CharacterCard, tattletale.CharacterCard);
            QuickHandStorage(alexandria);

            DealDamage(baron.CharacterCard, tattletale.CharacterCard, 2, DamageType.Melee);

            QuickHPCheck(-1, 0);
            QuickHandCheck(0);
        }

        [Test()]
        public void TestDamageWouldGoToZero()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "Megalopolis");

            StartGame();

            PlayCard("Invincible");

            DecisionYesNo = true;

            QuickHPStorage(alexandria.CharacterCard);
            QuickHandStorage(alexandria);

            DealDamage(baron.CharacterCard, alexandria.CharacterCard, 1, DamageType.Melee);

            QuickHPCheck(0);
            QuickHandCheck(0);
        }
    }
}
