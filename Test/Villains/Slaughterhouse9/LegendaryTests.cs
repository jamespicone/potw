using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Slaughterhouse9
{
    [TestFixture()]
    public class LegendaryTests : Slaughterhouse9TestBase
    {
        [Test()]
        public void TestIncreasesVillainDamage()
        {
            SetupNineGame();

            PutMemberInPlay("JackSlashCharacter");
            PlayCard("Legendary", 0);

            QuickHPStorage(haka);
            DealDamage(jackslash, haka.CharacterCard, 2, DamageType.Melee);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestDoesNotIncreaseHeroDamage()
        {
            SetupNineGame();

            PutMemberInPlay("JackSlashCharacter");
            PlayCard("Legendary", 0);

            QuickHPStorage(bunker);
            DealDamage(haka, bunker, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }
    }
}
