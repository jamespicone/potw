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
    public class TeleporterTests : CoilTestBase
    {
        [Test()]
        public void TestRevealsNonDeviceTargetsIntoPlay()
        {
            SetupCoilGame();
            RemoveCoilTriggers();
            CleanupSetupNoise();

            PlayCard("Teleporter", 0);

            // Top of deck (top first): I Know All Your Tricks, Mercenaries,
            // Sundancer, Trickster.
            var stacked = StackDeckHandleDuplicates(
                "Trickster", "Sundancer", "Mercenaries", "IKnowAllYourTricks").ToList();
            var trickster = stacked[0];
            var sundancer = stacked[1];
            var mercs = stacked[2];
            var oneShot = stacked[3];

            GoToEndOfTurn();
            GoToStartOfTurn(coil);

            // The first H = 3 non-device targets revealed were put into play;
            // the one-shot was shuffled back.
            AssertIsInPlay(mercs);
            AssertIsInPlay(sundancer);
            AssertIsInPlay(trickster);
            AssertInDeck(oneShot);
        }
    }
}
