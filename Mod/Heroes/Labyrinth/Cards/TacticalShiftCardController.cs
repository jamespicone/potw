using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;
using Handelabra;
using System.Reflection.Emit;

namespace Jp.ParahumansOfTheWormverse.Labyrinth
{
    public class TacticalShiftCardController : LabyrinthOneShotCardController
    {
        public TacticalShiftCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator UsualEffect()
        {
            // You may destroy an Ongoing card
            var e = GameController.SelectAndDestroyCard(
                HeroTurnTakerController,
                new LinqCardCriteria(c => c.Is().Ongoing().AccordingTo(this), "ongoing"),
                optional: true,
                cardSource: GetCardSource()
            );
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
