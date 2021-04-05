using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Tattletale
{
    public class ConfidenceCardController : CardController
    {
        public ConfidenceCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // "At the start of your turn, each hero regains 1 HP."
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => base.GameController.GainHP(base.HeroTurnTakerController, (Card c) => c.IsHero && c.IsCharacter && base.GameController.IsCardVisibleToCardSource(c, GetCardSource()), 1, cardSource: GetCardSource()), TriggerType.GainHP);
            base.AddTriggers();
        }
    }
}
