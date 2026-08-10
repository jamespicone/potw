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
            if (grueCharacter == null) { return false; }

            return (ds.IsCard && ds.Card == grueCharacter) || (ds.IsTurnTaker && ds.TurnTaker == grueCharacter.Owner);
        }

        // The {GrueCharacter} card this card belongs to. That's our turn taker's character card
        // normally, but there isn't one when The Celestial Tribunal's Representative of Earth
        // brings him in for the environment - then he's just another card in that play area.
        public static Card FindGrueCharacterCard(this CardController co)
        {
            if (co.CharacterCard != null) { return co.CharacterCard; }

            return co.GameController.FindCardsWhere(
                c => c.Owner == co.CardWithoutReplacements.Owner && c.IsInPlayAndHasGameText && c.IsHeroCharacterCard,
                realCardsOnly: false
            ).FirstOrDefault(c => co.GameController.FindCardController(c) is GrueCharacterCardController);
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
                var darknessCard = co.GameController.FindCardsWhere(c => c.IsGrueDarkness()).FirstOrDefault();

                var definition = darknessCard?.Definition;
                var owner = darknessCard?.Owner ?? co.CardWithoutReplacements.Owner;

                if (definition == null)
                {
                    // There are no Darkness cards anywhere to copy when The Celestial Tribunal's
                    // Representative of Earth brings {GrueCharacter} in from the box on his own,
                    // without his deck. Build one straight from the deck definition instead, owned
                    // by whoever owns him now.
                    definition = co.CardWithoutReplacements.Definition.ParentDeck
                        .GetAllCardDefinitions().FirstOrDefault(cd => cd.Identifier == "Darkness");

                    if (definition == null)
                    {
                        return co.GameController.SendMessageAction(
                            "There are no Darkness cards in the game.",
                            Priority.Medium,
                            co.GetCardSource(),
                            showCardSource: true
                        );
                    }
                }

                cardToMove = new Card(definition, owner, 0);
                var ownerController = co.FindTurnTakerController(owner);
                ownerController.TurnTaker.OutOfGame.AddCard(cardToMove);

                // The factory otherwise looks the controller up under the owning turn taker's deck,
                // which is the environment's once Representative of Earth owns {GrueCharacter}.
                var deck = definition.ParentDeck;
                var ns = string.IsNullOrEmpty(definition.Namespace) ? deck.Namespace : definition.Namespace;
                var cardNamespace = $"{ns}.{deck.Identifier}";

                var newController = CardControllerFactory.CreateInstance(cardToMove, ownerController, cardNamespace);
                ownerController.AddCardController(newController);

                if (ownerController.TurnTaker.DeckDefinition.Identifier != deck.Identifier)
                {
                    // Same journal entry Representative of Earth writes, so the controller is
                    // rebuilt in our namespace rather than the owner's on reload.
                    co.GameController.AddCardPropertyJournalEntry(
                        cardToMove,
                        "OverrideTurnTaker",
                        new List<string> { cardNamespace, definition.Identifier }
                    );
                }
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

        public static void SetGrueCanUseTriggerPowers(this CardController co)
        {
            co.SetCardPropertyToTrueIfRealAction("GrueSecondTrigger");
        }

        public static bool CanGrueUseTriggerPowers(this CardController co)
        {
            return co.GetCardPropertyJournalEntryBoolean("GrueSecondTrigger").GetValueOrDefault(false);
        }
    }
}
