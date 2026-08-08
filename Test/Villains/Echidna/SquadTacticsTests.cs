using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Echidna
{
    [TestFixture()]
    public class SquadTacticsTests : ParahumanTest
    {
        [Test()]
        public void TestWorks()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "InsulaPrimalis"
            );

            StartGame();
            ReturnAllTwisted();

            PlayCard("SquadTactics");

            QuickHPStorage(alexandria.CharacterCard);
            DealDamage(echidna.CharacterCard, alexandria.CharacterCard, 1, DamageType.Radiant);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestDoesNotIncreaseNonVillainDamage()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "InsulaPrimalis"
            );

            StartGame();
            ReturnAllTwisted();

            PlayCard("SquadTactics");

            // Hero damage is unchanged.
            QuickHPStorage(bitch.CharacterCard);
            DealDamage(alexandria.CharacterCard, bitch.CharacterCard, 1, DamageType.Melee);
            QuickHPCheck(-1);

            // Environment damage is unchanged.
            var raptor = PlayCard("VelociraptorPack");
            QuickHPStorage(alexandria.CharacterCard);
            DealDamage(raptor, alexandria.CharacterCard, 1, DamageType.Melee);
            QuickHPCheck(-1);
        }
    }
}
