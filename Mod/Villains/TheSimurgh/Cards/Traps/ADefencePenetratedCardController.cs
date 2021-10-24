using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.TheSimurgh
{
    public class ADefencePenetratedCardController : CardController
    {
        public ADefencePenetratedCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            return card == Card;
        }

        public override void AddTriggers()
        {
            // Damage that would be dealt to hero targets cannot be redirected.
            if (Card.IsFaceUp)
            {
                AddMakeDamageNotRedirectableTrigger(dda => dda.Target.Is().Hero().Target());
            }
        }
    }
}
