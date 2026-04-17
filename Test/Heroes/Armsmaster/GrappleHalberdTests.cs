using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Armsmaster
{
    [TestFixture()]
    public class GrappleHalberdTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();
        }

        [Test()]
        public void TestIsEquipmentHalberd()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            var card = GetCard("GrappleHalberd");
            Assert.That(card.DoKeywordsContain("equipment"), Is.True);
            Assert.That(card.DoKeywordsContain("halberd"), Is.True);
        }

        [Test()]
        public void TestPowerDestroysEnvironmentCard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            var envCard = PlayCard("VelociraptorPack");
            var halberd = PlayCard("GrappleHalberd");

            DecisionSelectCard = envCard;
            DecisionDoNotActivatableAbility = true;

            UsePower(halberd);

            AssertInTrash(envCard);
        }

        [Test()]
        public void TestPowerCanSkipDestruction()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            var envCard = PlayCard("VelociraptorPack");
            var halberd = PlayCard("GrappleHalberd");

            DecisionDoNotSelectCard = SelectionType.DestroyCard;
            DecisionDoNotActivatableAbility = true;

            UsePower(halberd);

            AssertIsInPlay(envCard);
        }

        [Test()]
        public void TestPowerActivatesModules()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            var halberd = PlayCard("GrappleHalberd");
            DecisionSelectWord = "Primary";
            var recycler = PlayCard("Recycler");
            AssertNextToCard(recycler, halberd);

            DecisionDoNotSelectCard = SelectionType.DestroyCard;
            DecisionActivateAbilities = new Card[] { recycler };

            QuickHandStorage(armsmaster);
            UsePower(halberd);
            // Recycler primary draws a card
            QuickHandCheck(1);
        }
    }
}
