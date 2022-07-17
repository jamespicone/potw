using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Echidna
{
    public class EngulfedCardController : CardController
    {
        public EngulfedCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowListOfCardsInPlay(
                new LinqCardCriteria(c => c.Is().Hero().Character() && !c.IsEngulfed(), "unengulfed hero")
            );
        }

        public override IEnumerator DeterminePlayLocation(
            List<MoveCardDestination> storedResults,
            bool isPutIntoPlay,
            List<IDecision> decisionSources,
            Location overridePlayArea = null,
            LinqTurnTakerCriteria additionalTurnTakerCriteria = null
        )
        {
            // Play this card next to a hero that does not already have an 'Engulfed' next to them.
            return SelectCardThisCardWillMoveNextTo(
                new LinqCardCriteria(
                    c =>
                        c.Is().Hero().Character() && ! c.IsEngulfed(),
                    "hero",
                    useCardsSuffix: true
                ),
                storedResults,
                isPutIntoPlay,
                decisionSources
            );
        }

        public override IEnumerator Play()
        {
            // If you do play the top card of the Twisted deck.
            return GameController.PlayTopCardOfLocation(TurnTakerController, TurnTaker.FindSubDeck("TwistedDeck"));
        }

        public override void AddTriggers()
        {
            // "If this card is not next to a hero, destroy it."
            AddIfTheTargetThatThisCardIsNextToLeavesPlayDestroyThisCardTrigger();

            // Whenever a power in this play area is used the target this card is next to deals themselves 2 psychic damage
            AddTrigger<UsePowerAction>(
                upa => upa.IsSuccessful && upa.Power.CardController.Card.Location.HighestRecursiveLocation == Card.Location.HighestRecursiveLocation,
                upa => DealDamage(GetCardThisCardIsNextTo(), GetCardThisCardIsNextTo(), 2, DamageType.Psychic),
                TriggerType.DealDamage,
                TriggerTiming.After
            );
        }
    }
}
