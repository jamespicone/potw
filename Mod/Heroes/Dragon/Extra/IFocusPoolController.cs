using System.Collections;

using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.Dragon
{
    public interface IFocusPoolController
    {
        IEnumerator AddFocusTokens(int tokens, CardSource source);

        IEnumerator LoseFocusTokens(int number, CardSource source);
    }
}
