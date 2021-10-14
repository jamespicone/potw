using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using UnityEngine;

namespace Jp.ParahumansOfTheWormverse.Grue
{
    public static class GrueExtensionMethods
    {
        public static bool IsGrueSource(this DamageSource ds, Card grueCharacter)
        {
            return (ds.IsCard && ds.Card == grueCharacter) || (ds.IsTurnTaker && ds.TurnTaker == grueCharacter.Owner);
        }

        // Puts a Darkness card into play next to 'target'
        // If a Darkness card is out of the game we use that one, otherwise one is synthesised
        public static IEnumerator PutDarknessIntoPlay(this CardController co, Card target)
        {
            var darknessOutOfPlay = co.GameController.FindCardsWhere(c => c.IsGrueDarkness() && c.Location.IsOutOfGame);

            Card cardToMove;
            if (darknessOutOfPlay.Count() > 0)
            {
                cardToMove = darknessOutOfPlay.First();
            }
            else
            {
                // Need to synthesise a card.
                var darknessCard = co.GameController.FindCardsWhere(c => c.IsGrueDarkness()).First();
                if (darknessCard == null)
                {
                    throw new InvalidOperationException("Couldn't find any darkness cards");
                }

                cardToMove = new Card(darknessCard.Definition, darknessCard.Owner, 0);
                var ownerController = co.FindTurnTakerController(darknessCard.Owner);
                ownerController.TurnTaker.OutOfGame.AddCard(cardToMove);
                
                var newController = CardControllerFactory.CreateInstance(cardToMove, ownerController);
                ownerController.AddCardController(newController);
            }

            return co.GameController.PlayCard(
                co.TurnTakerController,
                cardToMove,
                isPutIntoPlay: true,
                overridePlayLocation: target.NextToLocation,
                responsibleTurnTaker: co.TurnTaker,
                cardSource: co.GetCardSource()
            );
        }

        public static bool IsGrueDarkness(this Card c)
        {
            return c.Identifier == "Darkness" && c.Definition.ParentDeck.Namespace == "Jp.ParahumansOfTheWormverse";
        }

        public static bool DoesTargetHaveDarknessAdjacent(this CardController co, Card target)
        {
            if (target == null) { return false; }
            return target.GetAllNextToCards(false).Count(c => c.IsGrueDarkness()) > 0;
        }
    }
}
