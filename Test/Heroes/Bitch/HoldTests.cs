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
    public class HoldTests : ParahumanTest
    {
        [Test()]
        public void TestDestroysTargetWithinDogThreshold()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "InsulaPrimalis");
            StartGame();
            RemoveVillainCards();
            RemoveVillainTriggers();

            // Three dogs in play: X = 6 HP.
            PlayCard("Brutus");
            PlayCard("Judas");
            var angelica = PlayCard("Angelica");

            DecisionSelectCard = angelica;
            DecisionYesNo = false;

            PlayCard("Hold");

            AssertInTrash(angelica);
        }
    }
}
