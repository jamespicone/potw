using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Jp.ParahumansOfTheWormverse.Skitter;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Skitter
{
    [TestFixture()]
    public class TrackingBugsTests : ParahumanTest
    {
        [Test()]
        public void ZeroTokensOneRevealDiscardOptional()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            var stack = StackDeck("LivingForceField", "MobileDefensePlatform");

            PlayCard("TrackingBugs");

            DecisionSelectLocation = new LocationChoice(baron.TurnTaker.Deck);
            AssertDecisionIsOptional(SelectionType.DiscardCard);

            DecisionSelectCard = stack.Last();

            GoToEndOfTurn(skitter);

            AssertInTrash(stack.Last());
            AssertOnTopOfDeck(baron, stack.First());
        }

        [Test()]
        public void TwoTokensThreeReveals()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Skitter", "Jp.ParahumansOfTheWormverse.Tattletale", "Megalopolis");

            RemoveVillainTriggers();
            StartGame();
            RemoveMobileDefensePlatform();

            var stack = StackDeck("LivingForceField", "MobileDefensePlatform", "BladeBattalion", "HastenDoom").ToList();

            var bugs = PlayCard("TrackingBugs");
            var pool = bugs.FindBugPool();
            pool.AddTokens(2);

            DecisionSelectLocation = new LocationChoice(baron.TurnTaker.Deck);
            AssertDecisionIsOptional(SelectionType.DiscardCard);

            // Discard stack[2]; get order stack[1] // stack[3] // stack[0] // rest of deck
            var reorder = new List<Card>() { stack[2], stack[1], stack[3] };
            DecisionSelectCards = reorder;

            GoToEndOfTurn(skitter);

            AssertInTrash(reorder.First());
            AssertOnTopOfDeck(baron, reorder[1]);
            AssertOnTopOfDeck(baron, reorder[2], 1);
            AssertOnTopOfDeck(baron, stack[0], 2);
        }
    }
}
