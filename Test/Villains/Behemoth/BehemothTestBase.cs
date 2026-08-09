using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Behemoth
{
    public abstract class BehemothTestBase : ParahumanTest
    {
        protected Card heroTactics { get { return GetCard("HeroTacticsCharacter"); } }

        // The Movement deck lives under Behemoth's character card; used movement
        // cards go under the Movement Trash card.
        protected Location MovementDeck { get { return behemoth.CharacterCard.UnderLocation; } }
        protected Location MovementTrashPile { get { return GetCard("MovementTrash").UnderLocation; } }

        protected void SetupBehemothGame(bool advanced = false)
        {
            SetupGameController(
                new string[] { "Jp.ParahumansOfTheWormverse.Behemoth", "Legacy", "Bunker", "Haka", "Megalopolis" },
                advanced: advanced);
            StartGame();
        }

        protected TokenPool Proximity(TurnTakerController hero)
        {
            var marker = FindCardsWhere(
                c => c.Identifier == "Proximity" && c.Location == hero.TurnTaker.PlayArea,
                realCardsOnly: false).First();
            return marker.FindTokenPool("ProximityPool");
        }

        // Set a hero's proximity pool directly. A random movement card is played at
        // the start of villain turn 1, so pools are in an unknown state after StartGame.
        // Note this bypasses ModifyTokensAction, so token triggers don't fire.
        protected void SetProximity(TurnTakerController hero, int value)
        {
            Proximity(hero).SetNumberOfTokens(value);
        }

        // A hero's proximity marker leaves play when they are incapacitated, so their
        // cards can outlive their pool - including part-way through a card's effect.
        protected void RemoveProximityMarker(TurnTakerController hero)
        {
            var marker = FindCardsWhere(
                c => c.Identifier == "Proximity" && c.Location == hero.TurnTaker.PlayArea,
                realCardsOnly: false).First();
            // The marker is a non-real card, so it needs the model-level move.
            MoveCard(behemoth, marker, behemoth.TurnTaker.OutOfGame, overrideIndestructible: true);
        }

        protected void ClearProximity()
        {
            SetProximity(legacy, 0);
            SetProximity(bunker, 0);
            SetProximity(haka, 0);
        }

        // Remove the side triggers on Behemoth and Hero Tactics so individual cards
        // can be tested in isolation.
        protected void RemoveBehemothTriggers()
        {
            RemoveCardTriggers(behemoth.CharacterCard);
            RemoveCardTriggers(heroTactics);
        }

        // Put a specific movement card on top of the Movement deck.
        protected Card StackMovementDeck(string identifier, int index = 0)
        {
            var card = GetCard(identifier, index);
            MoveCard(behemoth, card, MovementDeck);
            return card;
        }

        protected Card PlayMovementCard(string identifier, int index = 0)
        {
            var card = GetCard(identifier, index);
            PlayCard(card);
            return card;
        }
    }
}
