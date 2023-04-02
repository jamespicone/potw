using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Slaughterhouse9
{
    public class CherishCharacterCardController : Slaughterhouse9MemberCharacterCardController
    {
        public CherishCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
        }

        public override void AddSideTriggers()
        {
            if (Card.IsFlipped)
            {
                // Whenever a target is destroyed during a hero's turn, that hero deals themselves 1 psychic damage
                AddSideTrigger(AddTrigger<DestroyCardAction>(
                    dca => dca.CardToDestroy.Card.IsTarget && dca.WasCardDestroyed && GameController.ActiveTurnTaker.Is(this).Hero(),
                    dca => TargetDestroyedResponse(dca),
                    TriggerType.DealDamage,
                    TriggerTiming.After
                ));
            }
            else
            {
                // The first time a Defence card would enter the trash each turn the hero with the most cards in hand discards 2 cards
                AddSideTrigger(AddDefenceTrigger(() => DiscardResponse(), new TriggerType[] { TriggerType.DiscardCard }, "CherishDiscard"));

                // Whenever Cherish has more than 0 HP and is dealt damage by a non-villain target, Cherish deals 2 psychic damage to that target
                AddSideTrigger(
                    AddCounterDamageTrigger(
                        dda => dda.Target == Card && dda.DamageSource.Is().NonVillain().Target().AccordingTo(this) && Card.HitPoints > 0,
                        () => Card,
                        () => Card,
                        oncePerTargetPerTurn: false,
                        2,
                        DamageType.Psychic
                    )
                );
            }
        }

        private IEnumerator TargetDestroyedResponse(DestroyCardAction dca)
        {
            var selectedCard = new List<Card>();
            var e = FindCharacterCardToTakeDamage(GameController.ActiveTurnTaker, selectedCard, Card, 1, DamageType.Psychic);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            if (selectedCard.Count <= 0) { yield break; }

            e = DealDamage(
                Card,
                selectedCard.First(),
                1,
                DamageType.Psychic,
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

        private IEnumerator DiscardResponse()
        {
            var selectedTurnTaker = new List<TurnTaker>();
            var e = FindHeroWithMostCardsInHand(selectedTurnTaker);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            if (selectedTurnTaker.Count <= 0) { yield break; }
            var selectedHeroController = FindHeroTurnTakerController(selectedTurnTaker.First() as HeroTurnTaker);
            if (selectedHeroController == null) { yield break; }

            e = GameController.SelectAndDiscardCards(
                selectedHeroController,
                2,
                optional: false,
                requiredDiscards: 2,
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
