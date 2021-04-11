
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.JessicaYamada
{
    class NothingIsLostCardController : CardController
    {
        public NothingIsLostCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            // "Each player may search their trash for a card and put it into their hand"
            var e = EachPlayerMovesCards(
                minNumberOfCards: 0,
                maxNumberOfCards: 1,
                SelectionType.MoveCardToHandFromTrash,
                new LinqCardCriteria((c) => true),
                (htt) => htt.Trash,
                (htt) => new List<MoveCardDestination>
                {
                    new MoveCardDestination(htt.Hand)
                }
            );

            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }
    }
}
