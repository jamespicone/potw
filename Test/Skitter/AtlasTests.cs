using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Jp.ParahumansOfTheWormverse.Skitter;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Skitter
{
    [TestFixture()]
    public class AtlasTests : BaseTest
    {
        protected HeroTurnTakerController skitter { get { return FindHero("Skitter"); } }
        protected HeroTurnTakerController tattletale { get { return FindHero("Tattletale"); } }

        [Test()]
        public void DrawCardAtEndOfTurn()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();

            PlayCard("Atlas");
            GoToDrawCardPhase(skitter);

            QuickHandStorage(skitter);

            EnterNextTurnPhase();

            QuickHandCheck(1);
        }

        [Test()]
        public void ImmuneToEnvDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();

            PlayCard("Atlas");
            var police = PlayCard("PoliceBackup");

            QuickHPStorage(skitter);
            DealDamage(police, skitter.CharacterCard, 1, DamageType.Infernal);
            QuickHPCheck(0);
        }
    }
}
