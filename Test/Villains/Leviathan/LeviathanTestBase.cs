using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Leviathan
{
    public abstract class LeviathanTestBase : ParahumanTest
    {
        protected void SetupLeviathanGame(bool advanced = false)
        {
            SetupGameController(
                new string[] { "Jp.ParahumansOfTheWormverse.Leviathan", "Legacy", "Bunker", "Haka", "Megalopolis" },
                advanced: advanced);
            StartGame();
        }

        // Setup puts a random Tactic into play. Move all tactics to the bottom of the
        // villain deck; nothing plays them from there, so tests control exactly which
        // tactics are active. (Don't move an in-play card through OutOfGame instead:
        // its triggers stay dead if it is later replayed.)
        protected void MoveTacticsToDeckBottom()
        {
            var tactics = FindCardsWhere(c => c.DoKeywordsContain("tactic")).ToList();
            MoveCards(leviathan, tactics, leviathan.TurnTaker.Deck, toBottom: true, overrideIndestructible: true);
        }

        // Put one tactic on top of the villain deck so a test can play it or a
        // flip can reveal it first.
        protected Card MoveTacticToDeckTop(string identifier)
        {
            var tactic = GetCard(identifier);
            MoveCard(leviathan, tactic, leviathan.TurnTaker.Deck, overrideIndestructible: true);
            return tactic;
        }

        // Ensure the given tactic is in play with live triggers and every other
        // tactic is inert at the bottom of the villain deck. If the tactic was
        // already in play from setup, moving it out and replaying it leaves its
        // triggers dead (the engine's AreTriggersActive guard gets out of sync),
        // so leave it where it is and explicitly re-register its triggers.
        protected Card PutTacticInPlay(string identifier)
        {
            var tactic = GetCard(identifier);
            var others = FindCardsWhere(c => c.DoKeywordsContain("tactic") && c != tactic).ToList();
            MoveCards(leviathan, others, leviathan.TurnTaker.Deck, toBottom: true, overrideIndestructible: true);

            if (!tactic.IsInPlayAndHasGameText)
            {
                MoveCard(leviathan, tactic, leviathan.TurnTaker.Deck, overrideIndestructible: true);
                PlayCard(tactic);
            }

            FindCardController(tactic).ResetTriggers(false);
            return tactic;
        }

        protected TokenPool RetaliationPool
        {
            get { return leviathan.CharacterCard.FindTokenPool("RetaliationPool"); }
        }
    }
}
