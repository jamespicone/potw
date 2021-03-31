using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Tattletale
{
    public class FollowsAllTheThreadsCardController : CardController
    {
        public FollowsAllTheThreadsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            // "Play with the top card of each deck face up."
            // Going to approximate this as "you may look at the top card of any deck at any time" to minimize weird interactions with, say, Ambuscade's Traps.
            IEnumerable<Location> allDecks = base.GameController.FindLocationsWhere((Location l) => l.IsDeck && l.IsRealDeck);
            foreach(Location deck in allDecks)
            {
                // Keeps track of the top card of every deck, but only DISPLAYS that info as long as this card is in play, its text is active, the deck has at least one card, and Tattletale can see that deck (e.g., she's not cut off from it by an effect like Isolated Hero or by being in the wrong battle zone)
                SpecialStringMaker.ShowSpecialString(() => "The top card of " + deck.Name.ToString() + " is " + deck.Cards.First().Title + ".").Condition = () => base.Card.IsInPlayAndHasGameText && deck.HasCards && base.GameController.IsTurnTakerVisibleToCardSource(deck.OwnerTurnTaker, GetCardSource());
            }
        }

        public override void AddTriggers()
        {
            // "At the start of your turn, {TattletaleCharacter} deals herself 1 psychic damage."
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => DealDamage(base.CharacterCard, base.CharacterCard, 1, DamageType.Psychic, cardSource: GetCardSource()), TriggerType.DealDamage);
            base.AddTriggers();
        }
    }
}
