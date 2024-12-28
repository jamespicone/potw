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
        public void TestWithRedirectAwayDiscard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            StartGame();

            PlayCard("Invincible");
            var wittyDeflection = PlayCard("WittyDeflection");

            DecisionYesNo = true;

            DecisionSelectCard = alexandria.CharacterCard;
            DecisionRedirectTarget = tattletale.CharacterCard;

            UsePower(wittyDeflection);

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
        public void TestWithRedirectAwayDontDiscard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            StartGame();

            PlayCard("Invincible");
            var wittyDeflection = PlayCard("WittyDeflection");

            DecisionYesNo = false;

            DecisionSelectCard = alexandria.CharacterCard;
            DecisionRedirectTarget = tattletale.CharacterCard;

            UsePower(wittyDeflection);

            QuickHPStorage(alexandria.CharacterCard, tattletale.CharacterCard);
            QuickHandStorage(alexandria);

            DealDamage(baron.CharacterCard, alexandria.CharacterCard, 2, DamageType.Melee);

            QuickHPCheck(0, -2);
            QuickHandCheck(0);
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
        public void TestWithRedirectAwayThenBackDiscard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            StartGame();

            PlayCard("Invincible");
            var wittyDeflection = PlayCard("WittyDeflection");

            DecisionsYesNo = new bool[] { false, true };

            DecisionSelectCard = tattletale.CharacterCard;
            UsePower(wittyDeflection);

            DecisionSelectCard = alexandria.CharacterCard;
            UsePower(wittyDeflection);

            DecisionRedirectTargets = new Card[] { tattletale.CharacterCard, alexandria.CharacterCard };         

            var cardToDiscard = alexandria.HeroTurnTaker.Hand.Cards.First();
            DecisionDiscardCard = cardToDiscard;

            QuickHPStorage(alexandria.CharacterCard, tattletale.CharacterCard);
            QuickHandStorage(alexandria);

            DealDamage(baron.CharacterCard, alexandria.CharacterCard, 2, DamageType.Melee);;

            QuickHPCheck(0, 0);
            QuickHandCheck(-1);

            AssertAtLocation(cardToDiscard, alexandria.TurnTaker.Trash);
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
