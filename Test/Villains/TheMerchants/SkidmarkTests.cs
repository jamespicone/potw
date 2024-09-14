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
    public class SkidmarkTests : ParahumanTest
    {
        [Test()]
        public void SkidmarkPlaysThugs()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "Legacy", "Parse", "InsulaPrimalis");

            StartGame();

            // Get rid of all of the villain cards for consistency
            MoveAllCards(merchants, merchants.TurnTaker.Deck, merchants.TurnTaker.OffToTheSide);

            GoToEndOfTurn(merchants);

            AssertNumberOfCardsInPlay(c => c.DoKeywordsContain("thug"), 2);
        }

        [Test()]
        public void PlaysSingleThugWhenFlipped()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "Legacy", "Parse", "InsulaPrimalis");

            StartGame();

            // Get rid of all of the villain cards for consistency
            MoveAllCards(merchants, merchants.TurnTaker.Deck, merchants.TurnTaker.OffToTheSide);

            GoToPlayCardPhase(merchants);

            PlayCard("Squealer");

            DealDamage(merchants, merchants, 100, DamageType.Infernal);

            AssertFlipped(merchants);

            GoToEndOfTurn(merchants);

            AssertNumberOfCardsInPlay(c => c.DoKeywordsContain("thug"), 1);
        }

        [Test()]
        public void HeroesWinIfNoVillainTargets()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "Legacy", "Parse", "InsulaPrimalis");

            StartGame();

            DealDamage(merchants, merchants, 100, DamageType.Infernal);

            AssertGameOver(EndingResult.AlternateVictory);
        }

        [Test()]
        public void SkidmarkFlipsWhenDestroyed()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "Legacy", "Parse", "InsulaPrimalis");

            StartGame();

            PlayCard("Squealer");

            DealDamage(merchants, merchants, 100, DamageType.Infernal);

            AssertFlipped(merchants);
            AssertNotGameOver();
        }

        [Test()]
        public void GameEndsIfSkidmarkFlippedAndLastTargetDestroyed()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "Legacy", "Parse", "InsulaPrimalis");

            StartGame();

            var squealer = PlayCard("Squealer");

            DealDamage(merchants, merchants, 100, DamageType.Infernal);

            AssertFlipped(merchants);
            AssertNotGameOver();

            DestroyCard(squealer);

            AssertGameOver(EndingResult.AlternateVictory);
        }

        [Test()]
        public void HeroesLoseIfNoThugsToPlay()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "Legacy", "Parse", "Haka", "Tachyon", "InsulaPrimalis");

            StartGame();

            // Get rid of all of the villain cards for consistency
            MoveAllCards(merchants, merchants.TurnTaker.Deck, merchants.TurnTaker.OffToTheSide);

            GoToEndOfTurn(merchants);

            AssertNumberOfCardsInPlay(c => c.DoKeywordsContain("thug"), 4);

            GoToEndOfTurn(merchants);

            AssertNumberOfCardsInPlay(c => c.DoKeywordsContain("thug"), 8);
            
            GoToEndOfTurn(merchants);

            AssertNumberOfCardsInPlay(c => c.DoKeywordsContain("thug"), 10);

            // All the thugs are in play now, so next start of villain turn the heroes lose

            GoToEndOfTurn(env);

            AssertNotGameOver();

            GoToStartOfTurn(merchants);

            AssertGameOver(EndingResult.AlternateDefeat);
        }

        [Test()]
        public void HeroesLoseIfNoThugsToPlayEvenIfSkidmarkIsFlipped()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "Legacy", "Parse", "Haka", "Tachyon", "InsulaPrimalis");

            StartGame();

            // Get rid of all of the villain cards for consistency
            MoveAllCards(merchants, merchants.TurnTaker.Deck, merchants.TurnTaker.OffToTheSide);

            GoToEndOfTurn(merchants);

            AssertNumberOfCardsInPlay(c => c.DoKeywordsContain("thug"), 4);

            GoToEndOfTurn(merchants);

            AssertNumberOfCardsInPlay(c => c.DoKeywordsContain("thug"), 8);

            GoToEndOfTurn(merchants);

            AssertNumberOfCardsInPlay(c => c.DoKeywordsContain("thug"), 10);

            // All the thugs are in play now, so next start of villain turn the heroes lose

            GoToEndOfTurn(env);

            AssertNotGameOver();

            DealDamage(merchants, merchants, 100, DamageType.Infernal);

            AssertFlipped(merchants);
            AssertNotGameOver();

            GoToStartOfTurn(merchants);

            AssertGameOver(EndingResult.AlternateDefeat);
        }

        [Test()]
        public void DestroyingAThugReturnsItToDeck()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "Legacy", "Parse", "Haka", "Tachyon", "InsulaPrimalis");

            StartGame();

            // Get rid of all of the villain cards for consistency
            MoveAllCards(merchants, merchants.TurnTaker.Deck, merchants.TurnTaker.OffToTheSide);

            GoToEndOfTurn(merchants);

            AssertNumberOfCardsInPlay(c => c.DoKeywordsContain("thug"), 4);

            GoToEndOfTurn(merchants);

            AssertNumberOfCardsInPlay(c => c.DoKeywordsContain("thug"), 8);

            GoToEndOfTurn(merchants);

            AssertNumberOfCardsInPlay(c => c.DoKeywordsContain("thug"), 10);

            // All the thugs are in play now, so next start of villain turn the heroes lose

            // Destroying a thug returns it to the thug deck
            DestroyCard("Tough");

            // So now we won't get a game over.

            GoToStartOfTurn(merchants);

            AssertNotGameOver();
        }

        [Test()]
        public void AdvancedReducesDamage()
        {
            SetupGameController(
                new string[] { "Jp.ParahumansOfTheWormverse.TheMerchants", "Tempest", "Legacy", "Parse", "InsulaPrimalis" },
                advanced: true);

            StartGame();

            QuickHPStorage(merchants);
            DealDamage(merchants, merchants, 1, DamageType.Melee);
            QuickHPCheck(0);
            DealDamage(merchants, merchants, 2, DamageType.Melee);
            QuickHPCheck(-1);
            DealDamage(tempest, merchants, 3, DamageType.Melee);
            QuickHPCheck(-2);
        }
    }
}
