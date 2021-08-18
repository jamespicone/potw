using System;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.Dragon
{
    public static class DragonExtensionMethods
    {
        public static IEnumerator AddFocusTokens(this CharacterCardController controller, int tokens, CardSource source)
        {
            var focusController = controller as IFocusPoolController;
            if (focusController == null) { yield break; }

            var e = focusController.AddFocusTokens(tokens, source);
            if (controller.UseUnityCoroutines)
            {
                yield return controller.GameController.StartCoroutine(e);
            }
            else
            {
                controller.GameController.ExhaustCoroutine(e);
            }
        }

        public static IEnumerator LoseFocusTokens(this CharacterCardController controller, int tokens, CardSource source)
        {
            var focusController = controller as IFocusPoolController;
            if (focusController == null) { yield break; }

            var e = focusController.LoseFocusTokens(tokens, source);
            if (controller.UseUnityCoroutines)
            {
                yield return controller.GameController.StartCoroutine(e);
            }
            else
            {
                controller.GameController.ExhaustCoroutine(e);
            }
        }
    }
}
