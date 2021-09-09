using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Dragon
{
    public class ParahumansOnlineCardController : CardController
    {
        public ParahumansOnlineCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            // At the start of your turn each player reveals the top card of their deck, then either returns it to the top of the deck or discards it
            AddStartOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => EachPlayerLooksAtTheTopCardOfTheirDeckThenReplacesItOrDiscardsItEx(TurnTakerController),
                TriggerType.RevealCard
            );
        }

        public override IEnumerator ActivateAbilityEx(CardDefinition.ActivatableAbilityDefinition ability)
        {
            if (ability.Name != "focus") { yield break; }

            // A player may draw a card
            var e = GameController.SelectHeroToDrawCard(
                HeroTurnTakerController,
                numberOfCards: 1,
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
        }
    }
}
