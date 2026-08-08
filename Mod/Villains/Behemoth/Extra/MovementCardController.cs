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
    public class MovementCardController : BehemothUtilityCardController
    {
        public const string MovementTrashIdentifier = "MovementTrash";

        public MovementCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            // Show all Proximity pools?
        }

        // Used movement cards go under the Movement Trash card, not the villain trash.
        // (Moving the card during Play() doesn't work: the engine's one-shot cleanup
        // moves it to the trash destination afterwards, so route that instead.)
        public override MoveCardDestination GetTrashDestination()
        {
            return new MoveCardDestination(base.TurnTaker.FindCard(MovementTrashIdentifier, realCardsOnly: false).UnderLocation);
        }
    }
}
