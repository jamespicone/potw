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
            SpecialStringMaker.ShowSpecialString(() => $"The top card of {TurnTaker.Deck.GetFriendlyName()} is {TurnTaker.Deck.TopCard.Title}.").Condition = () => Card.IsInPlayAndHasGameText && TurnTaker.Deck.HasCards;
        }

        public override void AddStartOfGameTriggers()
        {
            BuildTopDeckSpecialStrings();
        }

        private void BuildTopDeckSpecialStrings()
        {
            //this needs to be all turntakers in all zones.
            IEnumerable<TurnTaker> activeTurnTakers = FindTurnTakersWhere((TurnTaker tt) => !tt.IsIncapacitatedOrOutOfGame, true);
            foreach (TurnTaker tt in activeTurnTakers)
            {
                foreach (Location deck in tt.Decks.Where(deck => deck.IsRealDeck))
                {
                    var ss = SpecialStringMaker.ShowSpecialString(() => $"The top card of {deck.GetFriendlyName()} is {deck.TopCard.Title}.", relatedCards: () => tt.CharacterCards.Where(c => c.IsInPlayAndHasGameText));
                    ss.Condition = () => Card.IsInPlayAndHasGameText && deck.HasCards && GameController.IsLocationVisibleToSource(deck, GetCardSource());
                }
            }
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
            yield break;
        }
    }
}
