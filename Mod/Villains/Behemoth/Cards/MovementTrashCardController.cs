using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Behemoth
{
    public class MovementTrashCardController : BehemothUtilityCardController
    {
        public MovementTrashCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            if (card == base.Card)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void AddTriggers()
        {
            // The Movement trash is a trash, it's not in play
            base.Card.UnderLocation.OverrideIsInPlay = false;
            base.AddTriggers();
        }
    }
}
