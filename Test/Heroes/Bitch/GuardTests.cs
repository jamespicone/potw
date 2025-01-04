using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;
using Handelabra;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Bitch
{
    [TestFixture()]
    public class GuardTests : ParahumanTest
    {
        [Test()]
        public void NoDogs()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            MoveAllCardsFromHandToDeck(bitch);
            QuickHandStorage(bitch);

            DecisionDoNotSelectCard = SelectionType.ReduceDamageTaken;
            PlayCard("Guard");

            QuickHandCheck(1);
            AssertNumberOfStatusEffectsInPlay(0);
        }

        [Test()]
        public void OneDogsSkip()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            PlayCard("Milk");

            MoveAllCardsFromHandToDeck(bitch);
            QuickHandStorage(bitch);

            DecisionDoNotSelectCard = SelectionType.ReduceDamageTaken;
            PlayCard("Guard");

            QuickHandCheck(1);
            AssertNumberOfStatusEffectsInPlay(0);
        }

        [Test()]
        public void OneDogs()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            PlayCard("Milk");

            MoveAllCardsFromHandToDeck(bitch);
            QuickHandStorage(bitch);

            DecisionSelectCard = bitch.CharacterCard;
            PlayCard("Guard");

            QuickHandCheck(1);
            AssertNumberOfStatusEffectsInPlay(1);

            QuickHPStorage(bitch);
            DealDamage(baron, bitch, 3, DamageType.Melee);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TwoDogsSkipOne()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "Tempest", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            PlayCard("Milk");
            PlayCard("Kuro");

            MoveAllCardsFromHandToDeck(bitch);
            QuickHandStorage(bitch);

            DecisionSelectCards = new Card[] { bitch.CharacterCard, null };
            PlayCard("Guard");

            QuickHandCheck(1);
            AssertNumberOfStatusEffectsInPlay(1);

            QuickHPStorage(bitch, tempest);
            DealDamage(baron, bitch, 3, DamageType.Melee);
            DealDamage(baron, tempest, 3, DamageType.Melee);
            QuickHPCheck(-1, -3);
        }

        [Test()]
        public void TwoDogs()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Bitch", "Tempest", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            PlayCard("Milk");
            PlayCard("Kuro");

            MoveAllCardsFromHandToDeck(bitch);
            QuickHandStorage(bitch);

            DecisionSelectCards = new Card[] { bitch.CharacterCard, tempest.CharacterCard };
            PlayCard("Guard");

            QuickHandCheck(1);
            AssertNumberOfStatusEffectsInPlay(1);

            QuickHPStorage(bitch, tempest);
            DealDamage(baron, bitch, 3, DamageType.Melee);
            DealDamage(baron, tempest, 3, DamageType.Melee);
            QuickHPCheck(-1, -1);
        }
    }
}
