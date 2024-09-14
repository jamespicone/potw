using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.TheMerchants
{
    [TestFixture()]
    public class SquealerTests : ParahumanTest
    {
        [Test()]
        public void PlaysEnvironmentCards()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "Legacy", "Parse", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            GoToEndOfTurn(env);

            PlayCard("Squealer");

            var pack = StackDeck("VelociraptorPack");

            GoToStartOfTurn(merchants);

            AssertIsInPlay(pack);
        }

        [Test()]
        public void MakesImmuneToEnvironmentDamage()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "Legacy", "Parse", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            GoToEndOfTurn(env);

            var squealer = PlayCard("Squealer");

            var pack = PlayCard("VelociraptorPack");

            QuickHPStorage(squealer);
            DealDamage(pack, squealer, 5, DamageType.Melee, isIrreducible: true);
            QuickHPCheck(0);
        }

        [Test()]
        public void MakesImmuneToEnvironmentDamageFromNontargets()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "Legacy", "Parse", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            GoToEndOfTurn(env);

            var squealer = PlayCard("Squealer");

            var field = PlayCard("ObsidianField");

            QuickHPStorage(squealer);
            DealDamage(field, squealer, 5, DamageType.Melee, isIrreducible: true);
            QuickHPCheck(0);
        }
    }
}
