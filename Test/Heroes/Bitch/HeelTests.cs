using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Bitch
{
    [TestFixture()]
    public class HeelTests : ParahumanTest
    {
        private void SetupHeelGame()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "InsulaPrimalis");
            StartGame();
            RemoveVillainCards();
            RemoveVillainTriggers();
        }

        [Test()]
        public void TestFetchesDogFromDeck()
        {
            SetupHeelGame();

            var brutus = StackDeck("Brutus");

            DecisionSelectLocation = new LocationChoice(bitch.TurnTaker.Deck);
            DecisionYesNo = true;
            QuickHandStorage(bitch);

            PlayCard("Heel");

            AssertIsInPlay(brutus);
            // The optional draw was taken.
            QuickHandCheck(1);
        }

        [Test()]
        public void TestFetchesDogFromTrash()
        {
            SetupHeelGame();

            var judas = PutInTrash("Judas");

            DecisionSelectLocation = new LocationChoice(bitch.TurnTaker.Trash);
            DecisionYesNo = false;

            PlayCard("Heel");

            AssertIsInPlay(judas);
        }
    }
}
