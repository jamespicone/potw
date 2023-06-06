﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Jp.ParahumansOfTheWormverse.Tattletale;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Slaughterhouse9
{
    [TestFixture()]
    public class ShatterbirdTests : BaseTest
    {
        protected Card shatterbird { get { return FindCard(c => c.Identifier == "ShatterbirdCharacter"); } }

        protected HeroTurnTakerController alexandria { get { return FindHero("Alexandria"); } }

        [Test()]
        public void TestCanBeIncapped()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Megalopolis"
            );

            PlayCard("ShatterbirdCharacter");

            DealDamage(shatterbird, shatterbird, 30, DamageType.Melee);

            AssertFlipped(shatterbird);
        }
    }
}
