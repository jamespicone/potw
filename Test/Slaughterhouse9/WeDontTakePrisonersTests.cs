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
    public class NonlethalMeasuresTests : BaseTest
    {
        protected Card bonesaw { get { return FindCard(c => c.Identifier == "BonesawCharacter"); } }

        protected HeroTurnTakerController alexandria { get { return FindHero("Alexandria"); } }

        [Test()]
        public void TestDestroyedByDamage()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Legacy",
                "Tachyon",
                "InsulaPrimalis"
            );

            RemoveVillainCards();
            PlayCard("BonesawCharacter");
            var raptors = PlayCard("VelociraptorPack");
            PlayCard("WeDontTakePrisoners");

            QuickHPStorage(alexandria, legacy, tachyon);
            DealDamage(bonesaw, raptors, 20, DamageType.Infernal);
            QuickHPCheck(-2, -2, -2);
        }

        [Test()]
        public void TestDestroyedByDestroyEffect()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Legacy",
                "Tachyon",
                "InsulaPrimalis"
            );

            RemoveVillainCards();
            PlayCard("BonesawCharacter");
            var raptors = PlayCard("VelociraptorPack");
            PlayCard("WeDontTakePrisoners");

            QuickHPStorage(alexandria, legacy, tachyon);
            DestroyCard(raptors, bonesaw);
            QuickHPCheck(-2, -2, -2);
        }
    }
}
