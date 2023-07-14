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
    }
}
