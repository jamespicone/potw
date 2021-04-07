using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Kyushu
{
    public class Kyushu_LungCardController : CardController
    {
        public Kyushu_LungCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // "Reduce damage dealt to this card by 1."
            AddReduceDamageTrigger((Card c) => c == base.Card, 1);
            // "At the end of the environment turn, this card deals each target other than itself 3 fire damage."
            AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => c != base.Card, TargetType.All, 3, DamageType.Fire);
            // "At the start of the environment turn, this card regains 2 HP."
            AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => base.GameController.GainHP(base.Card, 2, cardSource: GetCardSource()), TriggerType.GainHP);
            base.AddTriggers();
        }
    }
}
