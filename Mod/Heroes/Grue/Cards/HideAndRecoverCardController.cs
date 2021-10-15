using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.Grue
{
    public class HideAndRecoverCardController : CardController
    {
        public HideAndRecoverCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            // "Put a Darkness card in play next to a hero character card.",
            var selectedCardList = new List<SelectCardDecision>();
            var e = GameController.SelectCardAndStoreResults(
                HeroTurnTakerController,
                SelectionType.MoveCardNextToCard,
                new LinqCardCriteria(c => c.Is().Hero().Character(), "hero character"),
                storedResults: selectedCardList,
                optional: false,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            var selectedCard = GetSelectedCard(selectedCardList);
            if (selectedCard == null) { yield break; }

            e = this.PutDarknessIntoPlay(selectedCard);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            // "That hero regains 2 HP...
            e = GameController.GainHP(selectedCard, 2, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            // and that hero's player may draw 2 cards"
            var player = selectedCard.Owner as HeroTurnTaker;
            if (player == null) { yield break; }

            var playerController = FindHeroTurnTakerController(player);
            if (playerController == null) { yield break; }

            e = GameController.DrawCards(playerController, 2, optional: true, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }
    }
}
