using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra;

using Jp.SOTMUtilities;
using Unity.IO.LowLevel.Unsafe;
using System.Security.Permissions;

namespace Jp.ParahumansOfTheWormverse.Labyrinth
{
    public abstract class LabyrinthOneShotCardController : CardController
    {
        public LabyrinthOneShotCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
        }

        public abstract IEnumerator UsualEffect();
        public abstract IEnumerator ShapingDestroyEffect();

        public sealed override IEnumerator Play()
        {
            // Do the usual effect of the one shot
            var e = UsualEffect();
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            // You may destroy a Shaping card
            var storedDestroy = new List<DestroyCardAction>();
            e = GameController.SelectAndDestroyCard(
                HeroTurnTakerController,
                new LinqCardCriteria(c => c.Is().WithKeyword("shaping").AccordingTo(this), "shaping"),
                optional: true,
                storedDestroy,
                responsibleCard: CharacterCard,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
            
            if (GetNumberOfCardsDestroyed(storedDestroy) > 0)
            {
                // If you do, do X
                e = ShapingDestroyEffect();
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }

            // "Put the top card of the environment deck into play."
            e = GameController.PlayTopCard(
                HeroTurnTakerController,
                FindEnvironment(),
                isPutIntoPlay: true,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }
    };
}
