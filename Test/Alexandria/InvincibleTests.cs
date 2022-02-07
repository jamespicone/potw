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
    public class InvincibleTests : BaseTest
    {
        protected HeroTurnTakerController alexandria { get { return FindHero("Alexandria"); } }

        [Test()]
        public void TestStatusEffect()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "Metropolis");

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
    }
}
