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
    public class LeapCardController : MovementCardController
    {
        public LeapCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroWithMostCards(false);
        }

        public override IEnumerator Play()
        {
            // "The player with the most cards in play puts 2 proximity tokens on their hero."
            List<TurnTaker> mostCardsResults = new List<TurnTaker>();
            IEnumerator findCoroutine = FindHeroWithMostCardsInPlay(mostCardsResults, ranking: 1, numberOfHeroes: 1);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(findCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(findCoroutine);
            }
            if (mostCardsResults.Count() > 0)
            {
                TurnTaker mostCardsTT = mostCardsResults.First();
                // Get their neighbors in turn order
                TurnTaker[] activeHeroTurnOrder = base.GameController.FindTurnTakersWhere((TurnTaker tt) => tt.IsHero && !tt.IsIncapacitatedOrOutOfGame).ToArray();
                int mostCardsIndex = Array.IndexOf(activeHeroTurnOrder, mostCardsTT);
                int prevIndex = (mostCardsIndex - 1 + activeHeroTurnOrder.Length) % activeHeroTurnOrder.Length;
                int nextIndex = (mostCardsIndex + 1 + activeHeroTurnOrder.Length) % activeHeroTurnOrder.Length;
                TurnTaker prevTT = activeHeroTurnOrder.ElementAt(prevIndex);
                TurnTaker nextTT = activeHeroTurnOrder.ElementAt(nextIndex);
                // Add 2 tokens to mostCardsTT's pool
                IEnumerator addTwoCoroutine = AddProximityTokens(mostCardsTT, 2, GetCardSource(), true);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(addTwoCoroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(addTwoCoroutine);
                }
                // "The players with active heroes immediately before and after that player in the turn order each put 1 proximity token on their hero."
                List<TurnTaker> neighbors = new List<TurnTaker>();
                neighbors.Add(prevTT);
                neighbors.Add(nextTT);
                IEnumerator addOneCoroutine = base.GameController.SelectTurnTakersAndDoAction(DecisionMaker, new LinqTurnTakerCriteria((TurnTaker tt) => neighbors.Contains(tt)), SelectionType.AddTokens, (TurnTaker tt) => AddProximityTokens(tt, 1, GetCardSource(), true), allowAutoDecide: true, numberOfCards: 1, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(addOneCoroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(addOneCoroutine);
                }
            }
            //Log.Debug("LeapCardController.Play() finished, passing to base.Play()");
            yield return base.Play();
        }
    }
}
