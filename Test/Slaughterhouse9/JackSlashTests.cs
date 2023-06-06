using System;
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
    public class JackSlashTests : BaseTest
    {
        protected Card jack { get { return FindCard(c => c.Identifier == "JackSlashCharacter"); } }

        protected HeroTurnTakerController alexandria { get { return FindHero("Alexandria"); } }

        [Test()]
        public void TestAttack()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Megalopolis"
            );

            PlayCard("JackSlashCharacter");

            QuickHPStorage(alexandria.CharacterCard);

            var prisoners = PlayCard("WeDontTakePrisoners");
            DestroyCard(prisoners);

            QuickHPCheck(-2);

            PlayCard("AlexandriasCape");

            prisoners = PlayCard("WeDontTakePrisoners");
            DestroyCard(prisoners);

            QuickHPCheck(-4);
        }

        [Test()]
        public void TestCanBeIncapped()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Megalopolis"
            );

            PlayCard("JackSlashCharacter");

            DealDamage(jack, jack, 30, DamageType.Melee);

            AssertFlipped(jack);
        }
    }
}
