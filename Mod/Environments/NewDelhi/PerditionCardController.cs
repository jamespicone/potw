using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.NewDelhi
{
    public class PerditionCardController : CardController
    {
        public PerditionCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // "At the start of the environment turn, play the top card of the villain trash."
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, PlayTopCardOfVillainTrashResponse, TriggerType.PlayCard);
            base.AddTriggers();
        }

        public IEnumerator PlayTopCardOfVillainTrashResponse(PhaseChangeAction pca)
        {
            // "... play the top card of the villain trash."
            List<SelectLocationDecision> trashChoice = new List<SelectLocationDecision>();
            IEnumerator selectCoroutine = base.GameController.SelectATrash(DecisionMaker, SelectionType.PlayTopCard, (Location l) => IsVillain(l.OwnerTurnTaker) && l.HasCards, trashChoice, noValidLocationsMessage: "There are no cards in any villain trash for " + base.Card.Title + " to play.", cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(selectCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(selectCoroutine);
            }
            if (DidSelectLocation(trashChoice))
            {
                Location villainTrash = trashChoice.FirstOrDefault().SelectedLocation.Location;
                IEnumerator playCoroutine = base.GameController.PlayTopCardOfLocation(base.TurnTakerController, villainTrash, responsibleTurnTaker: base.TurnTaker, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(playCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(playCoroutine);
                }
            }
            yield break;
        }
    }
}
