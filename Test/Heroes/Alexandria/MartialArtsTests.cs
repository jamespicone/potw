using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Alexandria
{
    [TestFixture()]
    public class MartialArtsTests : ParahumanTest
    {
        [Test()]
        public void TestCantDestroyHighHP()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "InsulaPrimalis");

            StartGame();

            var platform = GetMobileDefensePlatform().Card;
            PlayCard("MartialArts");

            AssertIsInPlay(platform);
        }

        [Test()]
        public void TestCantDestroyCharacters()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "InsulaPrimalis");

            StartGame();

            RemoveMobileDefensePlatform();

            SetHitPoints(baron.CharacterCard, 3);
            PlayCard("MartialArts");

            AssertIsInPlay(baron.CharacterCard);
        }

        [Test()]
        public void TestCanDestroyLowHP()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "InsulaPrimalis");

            StartGame();

            var platform = GetMobileDefensePlatform().Card;
            SetHitPoints(platform, 3);
            PlayCard("MartialArts");

            AssertInTrash(platform);
        }

        [Test()]
        public void TestDestroyEffectsCauseStatusEffect()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "InsulaPrimalis");

            StartGame();

            var platform = GetMobileDefensePlatform().Card;
            SetHitPoints(platform, 3);

            DecisionSelectCards = new Card[] { platform, baron.CharacterCard };
            PlayCard("MartialArts");

            AssertNumberOfStatusEffectsInPlay(1);

            QuickHPStorage(alexandria);
            DealDamage(baron.CharacterCard, alexandria.CharacterCard, 3, DamageType.Melee);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestDestroyByDamageCausesStatusEffect()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Alexandria", "InsulaPrimalis");

            StartGame();

            var platform = GetMobileDefensePlatform().Card;
            PlayCard("MartialArts");

            DecisionSelectCards = new Card[] { baron.CharacterCard };

            AssertNumberOfStatusEffectsInPlay(0);

            DealDamage(alexandria.CharacterCard, platform, 100, DamageType.Infernal);

            AssertNumberOfStatusEffectsInPlay(1);

            QuickHPStorage(alexandria);
            DealDamage(baron.CharacterCard, alexandria.CharacterCard, 3, DamageType.Melee);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestDestroyByDeathcaller()
        {
            SetupGameController("KaargraWarfang", "Jp.ParahumansOfTheWormverse.Alexandria", "InsulaPrimalis");

            MoveAllCards(warfang, warfang.TurnTaker.FindSubDeck("TitleDeck"), warfang.TurnTaker.OutOfGame);
            StartGame();
            RemoveVillainCards();

            var title = PlayCard("TitleDeathCaller");
            var target = PlayCard("OrrimHiveminded");
            
            SetHitPoints(target, 3);           
            PlayCard("MartialArts");

            AssertAtLocation(title, alexandria.CharacterCard.BelowLocation);
            AssertInTrash(target);

            var target2 = PlayCard("OrrimHiveminded");
            var target3 = PlayCard("SoulslayerPerith");
            SetHitPoints(target2, 10);

            DecisionAutoDecideIfAble = true;
            DecisionSelectCards = new Card[] { target3 };
            DealDamage(alexandria, target2, 9, DamageType.Infernal);
            AssertNumberOfStatusEffectsInPlay(1);
            AssertInTrash(target2);
        }
    }
}
