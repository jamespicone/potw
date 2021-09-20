using System.Collections.Generic;
using System.Collections;

using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.Legend
{
    public interface IEffectCardController
    {
        DealDamageAction TypicalDamageAction(IEnumerable<Card> targets);

        IEnumerator DoEffect(IEnumerable<Card> targets);
    }
}
