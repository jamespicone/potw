using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.MissMilitia
{
    [TestFixture()]
    public class MissMilitiaTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "InsulaPrimalis");
        }

        [Test()]
        public void TestHas26HP()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "InsulaPrimalis");
            StartGame();

            Assert.That(missmilitia.CharacterCard.MaximumHitPoints, Is.EqualTo(26));
        }

        [Test()]
        public void TestPowerRevealsAndPlaysWeapons()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            // Stack deck with a weapon and a non-weapon
            var pistol = PutOnDeck("Pistol");
            var coverFire = PutOnDeck("CoverFire");

            // Skip the optional weapon power use
            DecisionDoNotSelectCard = SelectionType.UsePower;

            UsePower(missmilitia);

            // Weapon should be in play, non-weapon should be in trash
            AssertIsInPlay(pistol);
            AssertInTrash(coverFire);
        }

        [Test()]
        public void TestPowerCanUseWeaponPower()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            // Stack deck with two weapons
            var pistol = PutOnDeck("Pistol");
            var machete = PutOnDeck("Machete");

            DecisionSelectTarget = baron.CharacterCard;

            QuickHPStorage(baron);
            UsePower(missmilitia);
            // Should have dealt damage from the weapon power used
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestIncap1HeroDeals2Projectile()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            IncapacitateCharacter(missmilitia.CharacterCard, baron.CharacterCard);

            DecisionSelectCard = bunker.CharacterCard;
            DecisionSelectTarget = baron.CharacterCard;

            QuickHPStorage(baron);
            UseIncapacitatedAbility(missmilitia, 0);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestIncap2DestroyEquipmentForDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var equipment = PlayCard("FlakCannon");

            IncapacitateCharacter(missmilitia.CharacterCard, baron.CharacterCard);

            DecisionSelectTurnTaker = bunker.TurnTaker;
            DecisionSelectCard = equipment;
            DecisionSelectTarget = baron.CharacterCard;

            QuickHPStorage(baron);
            UseIncapacitatedAbility(missmilitia, 1);
            QuickHPCheck(-4);
            AssertInTrash(equipment);
        }

        [Test()]
        public void TestIncap3PlayerPlaysCard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia", "Bunker", "InsulaPrimalis");
            StartGame();

            IncapacitateCharacter(missmilitia.CharacterCard, baron.CharacterCard);

            var cardToPlay = PutInHand("FlakCannon");

            DecisionSelectTurnTaker = bunker.TurnTaker;
            DecisionSelectCard = cardToPlay;

            UseIncapacitatedAbility(missmilitia, 2);

            AssertIsInPlay(cardToPlay);
        }

        [Test()]
        public void TestPromoHas28HP()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia/MissMilitiaProtectorateCaptainCharacter", "InsulaPrimalis");
            StartGame();

            Assert.That(missmilitia.CharacterCard.MaximumHitPoints, Is.EqualTo(28));
        }

        [Test()]
        public void TestPromoPowerActivatesAllEffects()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia/MissMilitiaProtectorateCaptainCharacter", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            var pistol = PlayCard("Pistol");

            // Promo power uses weapon power with all effects active, then returns weapon to hand
            // Pistol: 2 projectile + {smg} play a card (all effects active)
            DecisionSelectTarget = baron.CharacterCard;
            DecisionDoNotSelectCard = SelectionType.PlayCard;

            QuickHPStorage(baron);
            UsePower(missmilitia);
            QuickHPCheck(-2);

            // Weapon should be returned to hand
            AssertInHand(pistol);
        }

        [Test()]
        public void TestPromoIncap1HeroUsesPower()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia/MissMilitiaProtectorateCaptainCharacter", "Bunker", "InsulaPrimalis");
            StartGame();

            IncapacitateCharacter(missmilitia.CharacterCard, baron.CharacterCard);

            DecisionSelectTurnTaker = bunker.TurnTaker;

            UseIncapacitatedAbility(missmilitia, 0);
        }

        [Test()]
        public void TestPromoIncap2HeroDeals2Melee()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia/MissMilitiaProtectorateCaptainCharacter", "Bunker", "InsulaPrimalis");
            StartGame();

            RemoveMobileDefensePlatform();

            IncapacitateCharacter(missmilitia.CharacterCard, baron.CharacterCard);

            DecisionSelectCard = bunker.CharacterCard;
            DecisionSelectTarget = baron.CharacterCard;

            QuickHPStorage(baron);
            UseIncapacitatedAbility(missmilitia, 1);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestPromoIncap3HeroRegains2HP()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.MissMilitia/MissMilitiaProtectorateCaptainCharacter", "Bunker", "InsulaPrimalis");
            StartGame();

            SetHitPoints(bunker, 20);

            IncapacitateCharacter(missmilitia.CharacterCard, baron.CharacterCard);

            DecisionSelectCard = bunker.CharacterCard;

            QuickHPStorage(bunker);
            UseIncapacitatedAbility(missmilitia, 2);
            QuickHPCheck(2);
        }
    }
}
