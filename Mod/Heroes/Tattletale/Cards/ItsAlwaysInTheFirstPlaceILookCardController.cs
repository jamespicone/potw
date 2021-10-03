using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.Tattletale
{
    public class ItsAlwaysInTheFirstPlaceILookCardController : CardController
    {
        public ItsAlwaysInTheFirstPlaceILookCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            base.AddTriggers();
        }

        public override IEnumerator Play()
        {
            // "A player may search their deck for a card and put it into their hand, then shuffle their deck."
            // Choose a player
            List<SelectTurnTakerDecision> chosen = new List<SelectTurnTakerDecision>();
            IEnumerator chooseCoroutine = base.GameController.SelectTurnTaker(base.HeroTurnTakerController, SelectionType.SearchDeck, chosen, additionalCriteria: (TurnTaker tt) => this.HasAlignment(tt, CardAlignment.Hero) && GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource()) && tt.Deck.HasCards, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(chooseCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(chooseCoroutine);
            }
            if (DidSelectTurnTaker(chosen))
            {
                SelectTurnTakerDecision decision = chosen.FirstOrDefault();
                TurnTaker chosenPlayer = decision.SelectedTurnTaker;
                // That player fetches a card from deck to hand, then shuffles
                IEnumerator searchCoroutine = SearchForCards(FindHeroTurnTakerController(chosenPlayer.ToHero()), true, false, 1, 1, new LinqCardCriteria((Card c) => true), false, true, false, shuffleAfterwards: true);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(searchCoroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(searchCoroutine);
                }
            }
            yield break;
        }
    }
}
