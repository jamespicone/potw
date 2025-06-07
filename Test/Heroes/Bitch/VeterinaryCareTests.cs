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
    public class VeterinaryCareTests : ParahumanTest
    {
        [Test()]
        public void TestVeterinaryCareHealsAllDogs()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "InsulaPrimalis");

            StartGame();

            // Put some dogs into play
            Card brutus = PlayCard("Brutus");
            Card judas = PlayCard("Judas");
            Card angelica = PlayCard("Angelica");

            // Damage the dogs
            SetHitPoints(brutus, 2);
            SetHitPoints(judas, 3);
            SetHitPoints(angelica, 1);

            // Play Veterinary Care
            DecisionDoNotSelectCard = SelectionType.PlayCard;
            Card vetCare = PutInHand("VeterinaryCare");
            PlayCard(vetCare);

            // Verify all dogs are at full HP
            AssertIsAtMaxHP(brutus);
            AssertIsAtMaxHP(judas);
            AssertIsAtMaxHP(angelica);

            AssertIsInPlay(brutus, judas, angelica);
        }

        [Test()]
        public void TestVeterinaryCareDrawsAndPlaysCard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "InsulaPrimalis");
            StartGame();

            var brutus = PutInHand("Brutus");
            Card vetCare = PutInHand("VeterinaryCare");

            QuickHandStorage(bitch);

            DecisionSelectCardToPlay = brutus;
            PlayCard(vetCare);

            QuickHandCheck(-1);
            AssertIsInPlay(brutus);
        }
    }
}
