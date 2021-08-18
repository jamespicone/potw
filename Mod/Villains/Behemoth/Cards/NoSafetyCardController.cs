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
    public class NoSafetyCardController : MovementCardController
    {
        public NoSafetyCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            // "The player whose active hero has the fewest proximity tokens moves 1 proximity token from each other player's hero to their hero."
            // Find player with least tokens
            TurnTaker addingTT = null;
            IOrderedEnumerable<TokenPool> descendingProximities = OrderPoolsByHighestValue();
            int lastIndex = descendingProximities.Count() - 1;
            if (descendingProximities.Count() > 1 && !AllProximityPoolsEmpty())
            {
                if (descendingProximities.ElementAt(lastIndex).CurrentValue < descendingProximities.ElementAt(lastIndex - 1).CurrentValue)
                {
                    addingTT = TurnTakerForPool(descendingProximities.ElementAt(lastIndex));
                }
                else
                {
                    // There's a tie, figure it out
                    int lowestValue = descendingProximities.ElementAt(lastIndex).CurrentValue;
                    IEnumerable<TokenPool> tiedProximities = descendingProximities.Where((TokenPool tp) => tp.CurrentValue == lowestValue);
                    IEnumerable<TurnTaker> tiedTurnTakers = (from TokenPool tp in tiedProximities select TurnTakerForPool(tp));
                    List<SelectTurnTakerDecision> choice = new List<SelectTurnTakerDecision>();
                    IEnumerator selectCoroutine = base.GameController.SelectTurnTaker(DecisionMaker, SelectionType.RemoveTokens, choice, additionalCriteria: (TurnTaker tt) => tiedTurnTakers.Contains(tt), allowAutoDecide: true, cardSource: GetCardSource());
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
                        addingTT = choice.FirstOrDefault().SelectedTurnTaker;
                    }
                }
                // Each other player with at least 1 token passes a token to addingTT
                IEnumerable<TurnTaker> removingTurnTakers = base.GameController.FindTurnTakersWhere((TurnTaker tt) => tt.IsHero && !tt.IsIncapacitatedOrOutOfGame && tt != addingTT && ProximityPool(tt).CurrentValue > 0);
                IEnumerator passCoroutine = base.GameController.SelectTurnTakersAndDoAction(DecisionMaker, new LinqTurnTakerCriteria((TurnTaker tt) => removingTurnTakers.Contains(tt)), SelectionType.RemoveTokens, (TurnTaker tt) => PassProximityTokens(tt, addingTT, 1), allowAutoDecide: true, numberOfCards: 1, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(passCoroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(passCoroutine);
                }
            }
            //Log.Debug("NoSafetyCardController.Play() finished, passing to base.Play()");
            yield return base.Play();
        }
    }
}
