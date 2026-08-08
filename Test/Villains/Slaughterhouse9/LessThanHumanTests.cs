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
    public class LessThanHumanTests : Slaughterhouse9TestBase
    {
        [Test()]
        public void TestReducesDamageToVillainTargets()
        {
            SetupNineGame();

            PutMemberInPlay("JackSlashCharacter");
            PlayCard("LessThanHuman", 0);

            QuickHPStorage(jackslash);
            DealDamage(haka, jackslash, 3, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestDoesNotReduceDamageToHeroes()
        {
            SetupNineGame();

            PutMemberInPlay("JackSlashCharacter");
            PlayCard("LessThanHuman", 0);

            QuickHPStorage(haka);
            DealDamage(jackslash, haka.CharacterCard, 3, DamageType.Melee);
            QuickHPCheck(-3);
        }
    }
}
