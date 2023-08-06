using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Jp.ParahumansOfTheWormverse.Tattletale;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Tattletale
{
    [TestFixture()]
    public class WittyDeflectionTests : ParahumanTest
    {
        [Test()]
        public void TestBasicUse()
        {
            SetupGameController(
                "BaronBlade",
                "Jp.ParahumansOfTheWormverse.Tattletale",
                "Tachyon",
                "Megalopolis"
            );

            StartGame();

            DecisionSelectCard = tachyon.CharacterCard;

            var deflect = PlayCard("WittyDeflection");
            UsePower(deflect);

            DecisionRedirectTarget = tattletale.CharacterCard;

            QuickHPStorage(tachyon.CharacterCard, tattletale.CharacterCard);
            AssertDecisionIsOptional(SelectionType.RedirectDamage);

            DealDamage(baron.CharacterCard, tachyon.CharacterCard, 1, DamageType.Melee);

            QuickHPCheck(0, -1);
        }

        [Test()]
        public void TestAfterBeingDestroyed()
        {
            SetupGameController(
                "BaronBlade",
                "Jp.ParahumansOfTheWormverse.Tattletale",
                "Tachyon",
                "Megalopolis"
            );

            StartGame();

            DecisionSelectCard = tachyon.CharacterCard;

            var deflect = PlayCard("WittyDeflection");
            UsePower(deflect);
            DestroyCard(deflect);

            DecisionRedirectTarget = tattletale.CharacterCard;

            QuickHPStorage(tachyon.CharacterCard, tattletale.CharacterCard);
            AssertDecisionIsOptional(SelectionType.RedirectDamage);

            DealDamage(baron.CharacterCard, tachyon.CharacterCard, 1, DamageType.Melee);

            QuickHPCheck(0, -1);
        }
    }
}
