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
    public class SlipperyCardController : CardController
    {
        public SlipperyCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            // A player may discard their hand. If no cards were discarded, {EchidnaCharacter} is immune to damage until the start of her next turn.
            var dca = new List<DiscardCardAction>();
            var e = GameController.SelectHeroToDiscardTheirHand(
                DecisionMaker,
                optionalSelectHero: true,
                optionalDiscardCards: false,
                storedResultsDiscard: dca,
                responsibleTurnTaker: TurnTaker,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            if (! DidDiscardCards(dca))
            {
                var effect = new ImmuneToDamageStatusEffect();
                effect.TargetCriteria.IsSpecificCard = CharacterCard;
                effect.UntilCardLeavesPlay(CharacterCard);
                effect.UntilStartOfNextTurn(TurnTaker);

                e = AddStatusEffect(effect);
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }
        }
    }
}
