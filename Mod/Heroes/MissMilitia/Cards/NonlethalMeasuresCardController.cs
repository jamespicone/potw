using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.MissMilitia
{
    public class NonlethalMeasuresCardController : MissMilitiaUtilityCardController
    {
        public NonlethalMeasuresCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // "Whenever {MissMilitiaCharacter} destroys a non-character card target, you may put that target on the bottom of its deck."
            base.AddTrigger<DestroyCardAction>((DestroyCardAction dca) => dca.WasCardDestroyed && dca.CardToDestroy.Card.IsTarget && !dca.CardToDestroy.Card.IsCharacter && dca.ResponsibleCard.Owner.CharacterCard == base.CharacterCard, MoveToBottomOfDeckResponse, new TriggerType[] { TriggerType.DrawCard, TriggerType.PlayCard }, TriggerTiming.After);
            base.AddTriggers();
        }

        public IEnumerator MoveToBottomOfDeckResponse(DestroyCardAction dca)
        {
            // "... you may put that target on the bottom of its deck."
            if (dca.PostDestroyDestinationCanBeChanged)
            {
                List<YesNoCardDecision> answers = new List<YesNoCardDecision>();
                IEnumerator decideCoroutine = base.GameController.MakeYesNoCardDecision(base.HeroTurnTakerController, SelectionType.MoveCardOnBottomOfDeck, dca.CardToDestroy.Card, storedResults: answers, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(decideCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(decideCoroutine);
                }
                if (DidPlayerAnswerYes(answers))
                {
                    dca.SetPostDestroyDestination(dca.CardToDestroy.Card.NativeDeck, toBottom: true, answers.CastEnumerable<YesNoCardDecision, IDecision>(), postDestroyDestinationCanBeChanged: false, cardSource: GetCardSource());
                }
            }
            yield break;
        }
    }
}
