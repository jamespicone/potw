using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Slaughterhouse9
{
    public abstract class Slaughterhouse9TestBase : ParahumanTest
    {
        protected Card nineCharacter { get { return GetCard("Slaughterhouse9Character"); } }

        // Board-only setup: without StartGame no random members are deployed, and
        // all Nine are still in the villain deck for PlayCard to fetch. This is the
        // existing convention in the member tests.
        protected void SetupNineGame()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.Slaughterhouse9", "Legacy", "Bunker", "Haka", "Megalopolis");
        }

        // Full setup: H random members are deployed from under the Nine card.
        // Their trash-reaction triggers are removed for isolation.
        protected void SetupAndStartNineGame()
        {
            SetupNineGame();
            StartGame();
            RemoveVillainTriggers();
        }

        protected IEnumerable<Card> MembersInPlay
        {
            get
            {
                return FindCardsWhere(c => c.DoKeywordsContain("nine") && c.IsInPlayAndNotUnderCard);
            }
        }

        // BaseTest.PlayCard(string) removes a card's triggers before playing it,
        // expecting the play to re-add them. If the random start-of-game
        // deployment already put the member into play, the engine refuses the
        // replay and the member is left in play with its triggers permanently
        // dead. Use this instead of PlayCard for member character cards.
        protected Card PutMemberInPlay(string identifier)
        {
            var member = GetCard(identifier);
            if (member.IsInPlayAndHasGameText)
            {
                var controller = FindCardController(member);
                if (!controller.AreTriggersActive)
                {
                    controller.AddCardTriggers();
                }
            }
            else
            {
                PlayCard(member);
            }
            return member;
        }

        // Puts every unflipped deployed member except `keep` back under the Nine
        // card, so their trash reactions and statics can't interfere.
        protected void ReturnMembersExcept(Card keep = null)
        {
            foreach (var member in MembersInPlay.Where(c => c != keep && !c.IsFlipped).ToList())
            {
                MoveCard(nine, member, nine.CharacterCard.UnderLocation, overrideIndestructible: true);
            }
        }

        // Empties the villain deck and trash so crossing the villain play phase
        // plays nothing.
        protected void RemoveVillainDeck()
        {
            MoveCards(nine, nine.TurnTaker.Deck.Cards.ToList(), nine.TurnTaker.OutOfGame);
            MoveCards(nine, nine.TurnTaker.Trash.Cards.ToList(), nine.TurnTaker.OutOfGame);
        }
    }
}
