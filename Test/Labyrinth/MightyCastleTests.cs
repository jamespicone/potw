using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Jp.ParahumansOfTheWormverse.Labyrinth;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Labyrinth
{
    [TestFixture()]
    public class MightyCastleTests : ParahumanTest
    {
        [Test()]
        public void TestReducesDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Legacy", "InsulaPrimalis");

            StartGame();
            GoToUsePowerPhase(labyrinth);

            RemoveVillainCards();
            var battalion = PlayCard("BladeBattalion");

            PlayCard("ObsidianField");
            PlayCard("MightyCastle");

            DecisionSelectCard = legacy.CharacterCard;
            GoToEndOfTurn(labyrinth);

            AssertNumberOfStatusEffectsInPlay(1);

            QuickHPStorage(labyrinth, legacy);
            DealDamage(labyrinth, c => c.IsHeroCharacterCard, 2, DamageType.Infernal);
            QuickHPCheck(-2, -1);
        }
    }
}
