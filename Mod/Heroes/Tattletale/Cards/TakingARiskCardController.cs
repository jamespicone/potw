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
    public class TakingARiskCardController : CardController
    {
        public TakingARiskCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            base.AddTriggers();
        }

        public override IEnumerator Play()
        {
            yield break;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // "Put the top card of a deck into play."
            // Select a deck
            // Choose a deck
            List<SelectLocationDecision> chosen = new List<SelectLocationDecision>();
            IEnumerator chooseDeckCoroutine = base.GameController.SelectADeck(base.HeroTurnTakerController, SelectionType.PlayTopCard, (Location l) => true, chosen, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(chooseDeckCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(chooseDeckCoroutine);
            }
            Location deck = GetSelectedLocation(chosen);
            if (deck != null)
            {
                // Put its top card into play
                IEnumerator putCoroutine = base.GameController.PlayTopCardOfLocation(base.TurnTakerController, deck, isPutIntoPlay: true, responsibleTurnTaker: base.TurnTaker, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(putCoroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(putCoroutine);
                }
            }
            yield break;
        }
    }
}
