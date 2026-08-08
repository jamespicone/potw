using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Lung
{
    [TestFixture()]
    public class BakudaTests : LungTestBase
    {
        // Play Bakuda, finish the current villain turn, and stack the given card so it
        // will be revealed at the start of the next villain turn.
        private Card SetupBakudaReveal(string revealedIdentifier)
        {
            RemoveLungTriggers();
            RemoveEnvironmentDeck();

            PlayCard("Bakuda");

            GoToEndOfTurn();

            return StackDeck(revealedIdentifier);
        }

        [Test()]
        public void TestOneShotRevealedDealsFireDamage()
        {
            SetupLungGame();
            var smash = SetupBakudaReveal("Smash");
            var bakuda = GetCardInPlay("Bakuda");

            QuickHPStorage(legacy, bunker, haka);
            AssertNextRevealReveals(smash);
            AssertDamageSource(bakuda, bakuda, bakuda);
            AssertDamageType(DamageType.Fire, DamageType.Fire, DamageType.Fire);

            GoToStartOfTurn(lung);

            QuickHPCheck(-5, -5, -5);
        }

        [Test()]
        public void TestOngoingRevealedEachHeroDiscards()
        {
            SetupLungGame();
            var pyro = SetupBakudaReveal("Pyrokinesis");

            AssertNextRevealReveals(pyro);

            GoToStartOfTurn(lung);

            AssertNumberOfCardsInTrash(legacy, 1);
            AssertNumberOfCardsInTrash(bunker, 1);
            AssertNumberOfCardsInTrash(haka, 1);
        }

        [Test()]
        public void TestTargetRevealedHeroWithMostCardsDestroysTheirCards()
        {
            SetupLungGame();

            var presence = PlayCard("InspiringPresence");
            var sense = PlayCard("DangerSense");

            var thugs = SetupBakudaReveal("ABBThugs");

            AssertNextRevealReveals(thugs);
            QuickHPStorage(legacy, bunker, haka);

            GoToStartOfTurn(lung);

            // Legacy had the most cards in play, so all his noncharacter cards are destroyed.
            AssertInTrash(presence);
            AssertInTrash(sense);

            // The revealed target was shuffled back, not played.
            AssertInDeck(thugs);
            QuickHPCheck(0, 0, 0);
        }
    }
}
