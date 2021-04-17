using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Behemoth
{
    public class UnstoppableAdvanceCardController : BehemothUtilityCardController
    {
        public UnstoppableAdvanceCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // "At the end of the villain turn, move 1 proximity token from another hero to the hero with the most proximity tokens."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, HighestProximityTakesOneResponse, TriggerType.ModifyTokens);
            base.AddTriggers();
        }

        public override IEnumerator Play()
        {
            // "When this card enters play, change {BehemothCharacter}'s damage type to melee."
            IEnumerator meleeCoroutine = SetBehemothDamageType(base.Card, DamageType.Melee);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(meleeCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(meleeCoroutine);
            }
            yield break;
        }

        public IEnumerator HighestProximityTakesOneResponse(GameAction ga)
        {
            // "... move 1 proximity token from another hero to the hero with the most proximity tokens."
            // Find the pool with the most tokens
            TurnTaker takingTT = null;
            IOrderedEnumerable<TokenPool> descendingProximities = OrderPoolsByHighestValue();
            if (descendingProximities.Count() <= 1 || AllProximityPoolsEmpty())
            {
                yield break;
            }
            if (descendingProximities.ElementAt(0).CurrentValue > descendingProximities.ElementAt(1).CurrentValue)
            {
                takingTT = TurnTakerForPool(descendingProximities.ElementAt(0));
            }
            else
            {
                // There's a tie, figure it out
                int highestValue = descendingProximities.ElementAt(0).CurrentValue;
                IEnumerable<TokenPool> tiedProximities = descendingProximities.Where((TokenPool tp) => tp.CurrentValue == highestValue);
                IEnumerable<TurnTaker> tiedTurnTakers = (from TokenPool tp in tiedProximities select TurnTakerForPool(tp));
                List<SelectTurnTakerDecision> choice = new List<SelectTurnTakerDecision>();
                IEnumerator selectCoroutine = base.GameController.SelectTurnTaker(DecisionMaker, SelectionType.RemoveTokens, choice, additionalCriteria: (TurnTaker tt) => tiedTurnTakers.Contains(tt), cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(selectCoroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(selectCoroutine);
                }
                if (DidSelectTurnTaker(choice))
                {
                    takingTT = choice.FirstOrDefault().SelectedTurnTaker;
                }
            }
            if (takingTT != null)
            {
                IEnumerator takeCoroutine = TakeProximityTokens(takingTT, 1);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(takeCoroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(takeCoroutine);
                }
            }
            yield break;
        }
    }
}
