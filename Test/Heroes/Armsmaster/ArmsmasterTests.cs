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
    public class ArmsmasterTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();
        }

        [Test()]
        public void TestHas30HP()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            Assert.That(armsmaster.CharacterCard.MaximumHitPoints, Is.EqualTo(30));
        }

        [Test()]
        public void TestPowerRevealsEquipmentToHand()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            var halberd = PutOnDeck("OriginalHalberd");

            QuickHandStorage(armsmaster);
            UsePower(armsmaster);
            QuickHandCheck(1);

            AssertInHand(halberd);
        }

        [Test()]
        public void TestPowerShufflesBackNonEquipment()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            PutOnDeck("DualWielding");

            QuickHandStorage(armsmaster);
            UsePower(armsmaster);
            QuickHandCheck(0);
        }

        [Test()]
        public void TestIncap0TargetRegains2HP()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            IncapacitateCharacter(armsmaster.CharacterCard, baron.CharacterCard);

            SetHitPoints(bunker, 20);
            DecisionSelectCard = bunker.CharacterCard;

            QuickHPStorage(bunker);
            UseIncapacitatedAbility(armsmaster, 0);
            QuickHPCheck(2);
        }

        [Test()]
        public void TestIncap1Deals1IrreducibleEnergy()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();
            IncapacitateCharacter(armsmaster.CharacterCard, baron.CharacterCard);

            DecisionSelectTarget = baron.CharacterCard;

            QuickHPStorage(baron);
            UseIncapacitatedAbility(armsmaster, 1);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestIncap2SearchTrashForEquipment()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Armsmaster", "Bunker", "InsulaPrimalis");
            StartGame();

            IncapacitateCharacter(armsmaster.CharacterCard, baron.CharacterCard);

            // Put an equipment card in Bunker's trash
            var flakCannon = GetCard("FlakCannon");
            MoveCard(bunker, flakCannon, bunker.TurnTaker.Trash);

            DecisionSelectTurnTaker = bunker.TurnTaker;
            DecisionSelectCard = flakCannon;

            UseIncapacitatedAbility(armsmaster, 2);

            AssertInHand(flakCannon);
        }
    }
}
