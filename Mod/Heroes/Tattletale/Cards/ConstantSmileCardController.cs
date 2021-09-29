using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.Tattletale
{
    public class ConstantSmileCardController : CardController
    {
        public ConstantSmileCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // "At the start of your turn, {TattletaleCharacter} deals each non-hero target 1 psychic damage."
            base.AddDealDamageAtStartOfTurnTrigger(base.TurnTaker, base.CharacterCard, (Card c) => this.HasAlignment(c, CardAlignment.Nonhero, CardTarget.Target) && base.GameController.IsCardVisibleToCardSource(c, GetCardSource()), TargetType.All, 1, DamageType.Psychic);
            base.AddTriggers();
        }
    }
}
