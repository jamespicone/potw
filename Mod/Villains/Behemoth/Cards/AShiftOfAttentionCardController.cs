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
    public class AShiftOfAttentionCardController : MovementCardController
    {
        public AShiftOfAttentionCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            // "The player whose hero has the most proximity tokens removes all proximity tokens from their hero."
            // "The player with an active hero 2 turns before them in the turn order puts that many proximity tokens on their hero."
            // Find the pool with the most tokens
            TurnTaker removingTT = null;
            IOrderedEnumerable<TokenPool> descendingProximities = OrderPoolsByHighestValue();
            if (descendingProximities.Count() > 1 && !AllProximityPoolsEmpty())
            {
                if (descendingProximities.ElementAt(0).CurrentValue > descendingProximities.ElementAt(1).CurrentValue)
                {
                    removingTT = TurnTakerForPool(descendingProximities.ElementAt(0));
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
                        removingTT = choice.FirstOrDefault().SelectedTurnTaker;
                    }
                }
                // Find the active hero 2 turns earlier in turn order
                TurnTaker[] activeHeroTurnOrder = base.GameController.FindTurnTakersWhere((TurnTaker tt) => tt.IsHero && !tt.IsIncapacitatedOrOutOfGame).ToArray();
                int removingIndex = Array.IndexOf(activeHeroTurnOrder, removingTT);
                int addingIndex = -1;
                if (removingIndex >= 2)
                {
                    addingIndex = removingIndex - 2;
                }
                else
                {
                    addingIndex = removingIndex + activeHeroTurnOrder.Length - 2;
                }
                TurnTaker addingTT = activeHeroTurnOrder.ElementAt(addingIndex);
                // Remove all of removingTT's tokens and pass them to addingTT
                int tokensToRemove = ProximityPool(removingTT).CurrentValue;
                IEnumerator passCoroutine = PassProximityTokens(removingTT, addingTT, tokensToRemove);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(passCoroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(passCoroutine);
                }
            }
            Log.Debug("AShiftOfAttentionCardController.Play() finished, passing to base.Play()");
            yield return base.Play();
        }
    }
}
