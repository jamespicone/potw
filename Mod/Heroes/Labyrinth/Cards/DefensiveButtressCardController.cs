using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Labyrinth
{
    public class DefensiveButtressCardController : LabyrinthOneShotCardController
    {
        public DefensiveButtressCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator UsualEffect()
        {
            // "Until the start of your next turn reduce damage dealt to all hero targets by 1.",
            var effect = new ReduceDamageStatusEffect(1);
            effect.TargetCriteria.IsHero = true;
            effect.TargetCriteria.IsTarget = true;
            effect.UntilStartOfNextTurn(TurnTaker);

            var e = AddStatusEffect(effect);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }

        public override IEnumerator ShapingDestroyEffect()
        {
            // destroy an Environment card
            AddToTemporaryTriggerList(AddTrigger<DestroyCardAction>(
                dca => dca.CardSource?.Card == Card && dca.PostDestroyDestinationCanBeChanged && dca.WasCardDestroyed,
                MoveUnderLabyrinthInstead,
                TriggerType.ChangePostDestroyDestination,
                TriggerTiming.After,
                outOfPlayTrigger: true
            ));
            var e = GameController.SelectAndDestroyCard(
                HeroTurnTakerController,
                new LinqCardCriteria(c => c.Is().Environment(), "environment"),
                optional: false,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
            RemoveTemporaryTriggers();
        }

        private IEnumerator MoveUnderLabyrinthInstead(DestroyCardAction dca)
        {
            dca.SetPostDestroyDestination(CharacterCard.UnderLocation, cardSource: GetCardSource());
            yield break;
        }
    }
}
