using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Echidna
{
    [TestFixture()]
    public class SpearpointTests : ParahumanTest
    {
        private void SetupSpearpointGame()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "InsulaPrimalis"
            );
            StartGame();
            RemoveVillainTriggers();
            DestroyNonCharacterVillainCards();
            ReturnAllTwisted();
        }

        [Test()]
        public void TestRegainsHPWhenEquipmentEntersPlay()
        {
            SetupSpearpointGame();

            var spearpoint = PlayCard("SpearpointTwisted");
            SetHitPoints(spearpoint, 5);

            PlayCard("ProstheticEye");

            AssertHitPoints(spearpoint, 10);
        }

        [Test()]
        public void TestEndOfTurnDestroysHeroEquipment()
        {
            SetupSpearpointGame();

            PlayCard("SpearpointTwisted");
            var eye = PlayCard("ProstheticEye");

            GoToEndOfTurn();

            AssertInTrash(eye);
        }
    }
}
