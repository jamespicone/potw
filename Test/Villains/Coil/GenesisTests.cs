using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Coil
{
    [TestFixture()]
    public class GenesisTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.Coil", "Tempest", "InsulaPrimalis");
        }

        [Test()]
        public void DestroyedByEnvDamage()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Coil",
                "Legacy",
                "InsulaPrimalis"
            );

            StartGame();
            RemoveVillainCards();
            var genesis = PlayCard("Genesis");

            DealDamage(FindEnvironment().TurnTaker, genesis, 20, DamageType.Infernal);

            AssertInDeck(genesis);
        }
    }
}
