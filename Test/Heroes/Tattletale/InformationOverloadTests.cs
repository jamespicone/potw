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
    public class InformationOverloadTests : ParahumanTest
    {
        [Test()]
        public void TestSelfDamage()
        {
            SetupGameController(
                "BaronBlade",
                "Jp.ParahumansOfTheWormverse.Tattletale",
                "Jp.ParahumansOfTheWormverse.JessicaYamada",
                "Megalopolis"
            );

            StartGame();

            PlayCard("InformationOverload");
            PlayCard("SupportAndStability");

            var power1 = PlayCard("Reading");
            var power2 = PlayCard("Reading");
            var power3 = PlayCard("Reading");

            DecisionYesNo = true;

            QuickHPStorage(tattletale.CharacterCard);
            UsePower(power1);
            QuickHPCheck(0);
            UsePower(power2);
            QuickHPCheck(-1);
            UsePower(power3);
            QuickHPCheck(-1);
        }
    }
}
