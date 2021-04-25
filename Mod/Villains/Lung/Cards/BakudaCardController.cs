using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Jp.ParahumansOfTheWormverse.Lung
{
    public class BakudaCardController : CardController
    {
        public BakudaCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowHeroWithMostCards(false);
        }

        public override void AddTriggers()
        {
            /*
                "At the start of the villain turn, reveal the top card of the villain deck.",
                "If it is a One-Shot, {Bakuda} deals 5 fire damage to all non-villain targets.",
                "If it is an Ongoing, each hero discards 1 card.",
                "If it is a target, the hero with the most cards in play destroys all of them",
                "Shuffle the revealed card back into the villain deck"
            */

            AddStartOfTurnTrigger(tt => tt == TurnTaker, pca => DoBakudaThings(),
                new TriggerType[]{
                    TriggerType.RevealCard,
                    TriggerType.DealDamage,
                    TriggerType.DiscardCard,
                    TriggerType.DestroyCard
                }
            );
        }

        public IEnumerator DoBakudaThings()
        {
            var storedResults = new List<Card>();
            var e = GameController.RevealCards(TurnTakerController, TurnTaker.Deck, 1, storedResults, revealedCardDisplay: RevealedCardDisplay.ShowRevealedCards, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            var card = storedResults.FirstOrDefault();
            if (card != null)
            {
                // If it is a One-Shot, {Bakuda} deals 5 fire damage to all non-villain targets.
                if (card.IsOneShot)
                {
                    e = GameController.DealDamage(DecisionMaker, card, c => c.IsTarget && c.IsInPlay && !c.IsVillain, 5, DamageType.Fire, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(e);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(e);
                    }
                }

                // If it is an Ongoing, each hero discards 1 card.
                if (card.IsOngoing)
                {
                    e = GameController.EachPlayerDiscardsCards(1, 1, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(e);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(e);
                    }
                }

                // If it is a target, the hero with the most cards in play destroys all of them
                if (card.IsTarget)
                {
                    var victimList = new List<TurnTaker>();
                    e = FindHeroWithMostCardsInPlay(victimList, evenIfCannotDealDamage: true);
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(e);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(e);
                    }

                    var victim = victimList.FirstOrDefault();
                    if (victim != null)
                    {
                        var heroVictim = FindHeroTurnTakerController(victim.ToHero());
                        if (heroVictim != null)
                        {
                            e = GameController.DestroyCards(heroVictim, new LinqCardCriteria(c => c.Owner == victim && !c.IsCharacter), cardSource: GetCardSource());
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
            }

            e = CleanupRevealedCards(TurnTaker.Revealed, TurnTaker.Deck, shuffleAfterwards: true);
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
