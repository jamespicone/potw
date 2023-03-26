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
        }

        public IEnumerator FlipOverCards()
        {
            var decks = FindLocationsWhere(l => l.IsDeck);
            foreach (var deck in decks)
            {
                if (deck.NumberOfCards > 0 && ! deck.TopCard.IsFaceUp)
                {
                    deck.TopCard.SetFaceUp(true);
                    deck.TopCard.SetIsPositionKnown(true);
                }
            }
            yield break;
        }

        public override IEnumerator Play()
        {
            return FlipOverCards();
        }

        public override void AddTriggers()
        {
            // "At the start of your turn, {TattletaleCharacter} deals herself 1 psychic damage."
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => DealDamage(base.CharacterCard, base.CharacterCard, 1, DamageType.Psychic, cardSource: GetCardSource()), TriggerType.DealDamage);

            AddTrigger<GameAction>(ga => ga.CardSource != GetCardSource(), (a) => FlipOverCards(), TriggerType.Hidden, TriggerTiming.After);
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
