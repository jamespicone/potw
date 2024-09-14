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
    public class FestivalOfBloodTests : ParahumanTest
    {
        [Test()]
        public void TestWorks()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "InsulaPrimalis");

            StartGame();

            PlayCard("FestivalOfBlood");

            AssertSourceDamageModified(new Card[] { merchants.CharacterCard }, new int[] { 1 }, tempest.CharacterCard);
        }
    }
}
