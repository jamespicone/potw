using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.UnitTest.MissMilitia
{
    [TestFixture()]
    public class NonlethalMeasuresTests : ParahumanTest
    {
        [Test()]
        public void TestDestroyedByDamage()
        {
            SetupGameController(
                "BaronBlade",
                "Jp.ParahumansOfTheWormverse.MissMilitia",
                "Legacy",
                "Tachyon",
                "InsulaPrimalis"
            );

            RemoveVillainCards();

            PlayCard("NonlethalMeasures");
            var raptors = PlayCard("VelociraptorPack");

            DecisionYesNo = true;
            DealDamage(missmilitia, raptors, 20, DamageType.Infernal);

            AssertOnBottomOfDeck(raptors);
        }

        [Test()]
        public void TestDestroyedByEnvDamage()
        {
            SetupGameController(
                "BaronBlade",
                "Jp.ParahumansOfTheWormverse.MissMilitia",
                "Legacy",
                "Tachyon",
                "InsulaPrimalis"
            );

            RemoveVillainCards();

            PlayCard("NonlethalMeasures");
            var raptors = PlayCard("VelociraptorPack");

            DecisionYesNo = true;
            DealDamage(FindEnvironment().TurnTaker, raptors, 20, DamageType.Infernal);

            AssertInTrash(raptors);
        }

        [Test()]
        public void TestDestroyedByDestroyEffect()
        {
            SetupGameController(
                "BaronBlade",
                "Jp.ParahumansOfTheWormverse.MissMilitia",
                "Legacy",
                "Tachyon",
                "InsulaPrimalis"
            );

            RemoveVillainCards();

            PlayCard("NonlethalMeasures");
            var raptors = PlayCard("VelociraptorPack");

            DecisionYesNo = true;
            DestroyCard(raptors, missmilitia.CharacterCard);

            AssertOnBottomOfDeck(raptors);
        }
    }
}
