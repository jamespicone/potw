using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.Coil
{
    public class TricksterCardController : CardController
    {
        public TricksterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // "The first time Trickster would be dealt damage each round redirect it to the hero target with the lowest HP",
            AddTrigger<DealDamageAction>(
                dda => dda.Target == Card && HasBeenSetToTrueThisRound("TricksterRedirect"),
                dda => DoRedirect(dda),
                TriggerType.RedirectDamage,
                TriggerTiming.Before
            );

            // "At the end of the villain turn the player with the most cards in play destroys one of their noncharacter cards"
            AddEndOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => DestroyHeroCard(),
                TriggerType.DestroyCard
            );
        }

        private IEnumerator DoRedirect(DealDamageAction dda)
        {
            SetCardPropertyToTrueIfRealAction("TricksterRedirect", gameAction: dda);

            return RedirectDamage(dda, TargetType.LowestHP, c => c.IsHeroTarget());
        }

        private IEnumerator DestroyHeroCard()
        {
            var storedResults = new List<TurnTaker>();
            var e = FindHeroWithMostCardsInPlay(storedResults, mostFewestSelectionType: SelectionType.DestroyCard);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            var hero = storedResults.FirstOrDefault() as HeroTurnTaker;
            if (hero == null) { yield break; }

            var httc = FindHeroTurnTakerController(hero);
            if (httc == null) { yield break; }

            e = GameController.SelectAndDestroyCard(
                httc,
                new LinqCardCriteria(c => ! c.IsCharacter && c.Owner == httc.TurnTaker),
                optional: false,
                responsibleCard: Card,
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
