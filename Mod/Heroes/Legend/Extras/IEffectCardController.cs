using System.Collections.Generic;
using System.Collections;

using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.Legend
{
    public enum EffectTargetingOrdering
    {
        OrderingAlreadyDecided,
        NeedsOrdering
    };

    public interface IEffectCardController
    {
        DealDamageAction TypicalDamageAction(IEnumerable<Card> targets, CardController sourceCard, CardSource cardSource);

        IEnumerator DoEffect(IEnumerable<Card> targets, CardController sourceCard, CardSource cardSourceToUse, EffectTargetingOrdering ordering = EffectTargetingOrdering.NeedsOrdering );
    }
}
