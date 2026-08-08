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
    public class LetsTrySomethingElseTests : CoilTestBase
    {
        [Test()]
        public void TestShufflesTrashIntoDeckAndPlays()
        {
            SetupCoilGame();
            RemoveCoilTriggers();
            CleanupSetupNoise();

            // Empty the villain deck and trash so the play is deterministic:
            // the trash contains only Mercenaries.
            MoveCards(coil, coil.TurnTaker.Trash.Cards.ToList(), coil.TurnTaker.OutOfGame);
            var letsTry = GetCard("LetsTrySomethingElse");
            MoveCards(coil, coil.TurnTaker.Deck.Cards.Where(c => c != letsTry).ToList(), coil.TurnTaker.OutOfGame);

            var mercs = PutInTrash("Mercenaries");

            PlayCard(letsTry);

            // The trash was shuffled into the deck and its top card played.
            AssertIsInPlay(mercs);
            AssertInTrash(letsTry);
        }
    }
}
