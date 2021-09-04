using System.Collections.Generic;
using System.Collections;

using Handelabra.Sentinels.Engine.Model;


namespace Jp.ParahumansOfTheWormverse.Legend
{
    public interface IEffectCardController
    {
        IEnumerator DoEffect(IEnumerable<Card> targets);
    }
}
