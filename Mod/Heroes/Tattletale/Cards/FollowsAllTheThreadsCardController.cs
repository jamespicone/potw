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
    public class FollowsAllTheThreadsCardController : CardController
    {
        public FollowsAllTheThreadsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            // "Play with the top card of each deck face up."
            // Going to approximate this as "you may look at the top card of any deck at any time" to minimize weird interactions with, say, Ambuscade's Traps.
            SpecialStringMaker.ShowListOfCards(new LinqCardCriteria((Card c) => c.Location.IsDeck && c == c.Location.TopCard && base.GameController.IsLocationVisibleToSource(c.Location, GetCardSource()), "top cards of decks", false, false, "top card of deck", "top cards of decks"), () => base.Card.IsInPlayAndHasGameText).Condition = () => base.Card.IsInPlayAndHasGameText;
        }

        public override void AddTriggers()
        {
            // "At the start of your turn, {TattletaleCharacter} deals herself 1 psychic damage."
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => DealDamage(base.CharacterCard, base.CharacterCard, 1, DamageType.Psychic, cardSource: GetCardSource()), TriggerType.DealDamage);
            base.AddTriggers();
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // "Destroy this card."
            IEnumerator destroyCoroutine = base.GameController.DestroyCard(base.HeroTurnTakerController, base.Card, responsibleCard: base.Card, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(destroyCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(destroyCoroutine);
            }
            // ...
            yield break;
        }
    }
}
