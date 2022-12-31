using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Skitter
{
    public class AlwaysPlanningCardController : CardController
    {
        public AlwaysPlanningCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            // "At the start of the environment turn place a Bug token on {SkitterCharacter}."
            AddStartOfTurnTrigger(tt => tt.Is().Environment(), pca => this.AddBugTokenToSkitter(1), TriggerType.AddTokensToPool);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // "Reveal cards from the top of your deck until you reveal a Strategy card. Put it into play and shuffle the other cards back into your deck."
            return RevealCards_MoveMatching_ReturnNonMatchingCards(
                TurnTakerController,
                TurnTaker.Deck,
                playMatchingCards: false,
                putMatchingCardsIntoPlay: true,
                moveMatchingCardsToHand: false,
                new LinqCardCriteria(c => c.Is().WithKeyword("strategy").AccordingTo(this), "strategy"),
                numberOfMatches: 1,
                revealedCardDisplay: RevealedCardDisplay.ShowMatchingCards
            );
        }
    }
}
