using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Labyrinth
{
    public class GiantChessboardCardController : ShapingCardController
    {
        public GiantChessboardCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddShapingTriggers()
        {
            // "At the end of {LabyrinthCharacter}'s turn...
            AddEndOfTurnTrigger(tt => tt == TurnTaker, MaybeDealDamage, TriggerType.DealDamage);
        }

        public IEnumerator MaybeDealDamage(PhaseChangeAction pca)
        {
            // ...you may destroy an Environment card.
            var storedResults = new List<DestroyCardAction>();
            var e = GameController.SelectAndDestroyCard(
                HeroTurnTakerController,
                new LinqCardCriteria(c => c.Is().Environment(), "environment"),
                optional: true,
                storedResults,
                responsibleCard: CharacterCard,
                GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            // If you do...
            if (! DidDestroyCard(storedResults)) yield break;

            // ...this card deals X melee damage to a target where X is the number of shaping cards in play."
            e = GameController.SelectTargetsAndDealDamage(
                HeroTurnTakerController,
                new DamageSource(GameController, Card),
                c => ShapingCardCount(),
                DamageType.Melee,
                () => 1,
                optional: false,
                requiredTargets: null,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }

        private int ShapingCardCount()
        {
            return FindCardsWhere(
                c => c.Is().WithKeyword("shaping").AccordingTo(this) && c.IsInPlay,
                realCardsOnly: true,
                visibleToCard: GetCardSource()
            ).Count();
        }
    }
}
