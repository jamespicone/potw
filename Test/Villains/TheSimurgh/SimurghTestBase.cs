using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.TheSimurgh
{
    public abstract class SimurghTestBase : ParahumanTest
    {
        protected void SetupSimurghGame(bool advanced = false)
        {
            SetupGameController(
                new string[] { "Jp.ParahumansOfTheWormverse.TheSimurgh", "Legacy", "Bunker", "Haka", "Megalopolis" },
                advanced: advanced);
            StartGame();
        }

        protected Location ConditionDeck { get { return simurgh.TurnTaker.FindSubDeck("ConditionDeck"); } }

        protected IEnumerable<Card> FaceDownTrapsInPlay
        {
            get
            {
                // Face-down cards expose no keywords, so identify them by owner and
                // the engine's face-down flag (only traps are ever face down in play).
                return FindCardsWhere(c => c.Owner == simurgh.TurnTaker && c.IsInPlayAndNotUnderCard && c.IsFaceDownNonCharacter);
            }
        }

        protected void RemoveSimurghTriggers()
        {
            RemoveCardTriggers(simurgh.CharacterCard);
        }

        // Thinker Countermeasures cancel trap flips; get them out of the way.
        protected void RemoveCountermeasures()
        {
            var countermeasures = FindCardsWhere(c => c.Identifier == "ThinkerCountermeasures" && c.IsInPlay).ToList();
            MoveCards(simurgh, countermeasures, simurgh.TurnTaker.OutOfGame);
        }

        // Setup puts 4 random traps face down in play and the other 3 face up off to
        // the side. Make sure a specific trap is among the face-down ones.
        protected Card PutTrapFaceDownInPlay(string identifier)
        {
            var trap = GetCard(identifier);
            if (!(trap.IsInPlayAndNotUnderCard && trap.IsFlipped))
            {
                if (!trap.IsFlipped)
                {
                    FlipCard(trap);
                }
                MoveCard(simurgh, trap, simurgh.TurnTaker.PlayArea, playIfPlayArea: false, overrideIndestructible: true);
            }
            return trap;
        }

        // Flip a trap face up WITHOUT running its when-flipped effect, then activate
        // its passive triggers (a plain harness flip leaves them dormant).
        protected Card FlipTrapFaceUpDormant(string identifier)
        {
            var trap = PutTrapFaceDownInPlay(identifier);
            RemoveCountermeasures();
            FlipCard(trap);
            FindCardController(trap).ResetTriggers(false);
            return trap;
        }

        // Flip a face-down trap the way the game does it — via A Plan Enacted —
        // so its when-flipped effect runs. Every other face-down trap is flipped
        // up dormant first, making the flip choice unique: no decision properties
        // are needed, so the trap's own when-flipped decisions stay unpolluted.
        protected Card FlipTrapWithPlanEnacted(string identifier)
        {
            var trap = PutTrapFaceDownInPlay(identifier);
            RemoveCountermeasures();

            foreach (var other in FaceDownTrapsInPlay.Where(c => c != trap).ToList())
            {
                FlipCard(other);
            }

            PlayCard("APlanEnacted");
            return trap;
        }
    }
}
