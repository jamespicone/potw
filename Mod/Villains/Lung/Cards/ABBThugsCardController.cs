using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Lung
{
    public class ABBThugsCardController : CardController
    {
        public ABBThugsCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override System.Collections.IEnumerator Play()
        {
            // At the end of the villain turn, ABB Thugs deal the hero target with the lowest HP 2 projectile damage
            yield break;
        }
    }
}
