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
    public class FestivalOfExcessTests : ParahumanTest
    {
        [Test()]
        public void TestWorks()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "InsulaPrimalis");

            StartGame();

            PlayCard("FestivalOfExcess");

            AssertSourceDamageModified(new Card[] { tempest.CharacterCard }, new int[] { -1 }, merchants.CharacterCard);
        }
    }
}
