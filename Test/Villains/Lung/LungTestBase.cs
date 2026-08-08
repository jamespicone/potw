using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Lung
{
    public abstract class LungTestBase : ParahumanTest
    {
        protected Card brute { get { return GetCard("BruteInstructions"); } }

        protected void SetupLungGame(bool advanced = false)
        {
            SetupGameController(
                new string[] { "Jp.ParahumansOfTheWormverse.Lung", "Legacy", "Bunker", "Haka", "Megalopolis" },
                advanced: advanced);
            StartGame();
        }

        // Remove the side triggers on Lung and the Brute instructions card so individual
        // villain cards can be tested without the end-of-turn machinery interfering.
        protected void RemoveLungTriggers()
        {
            RemoveCardTriggers(lung.CharacterCard);
            RemoveCardTriggers(brute);
        }

        // Move n villain cards from the deck to the trash.
        // The pool deliberately excludes Wings, White-Hot Flame and Pyrokinesis (reserved as
        // harmless cards for tests to stack on the deck), and puts Smash / Burst of Flame
        // last so tests exercising those cards can still fetch a deck copy for n <= 10.
        protected void FillLungTrash(int n)
        {
            var pool = new List<string>();
            void AddCopies(string id, int count)
            {
                for (int i = 0; i < count; i++) { pool.Add(id); }
            }

            AddCopies("ABBThugs", 3);
            AddCopies("TerribleBurns", 3);
            AddCopies("GrowingAnger", 2);
            AddCopies("Bakuda", 1);
            AddCopies("OniLee", 1);
            AddCopies("Smash", 4);
            AddCopies("BurstOfFlame", 4);

            Assert.That(n, Is.LessThanOrEqualTo(pool.Count), "FillLungTrash can only fill up to 18 cards");

            var counts = new Dictionary<string, int>();
            foreach (var id in pool.Take(n))
            {
                counts.TryGetValue(id, out var index);
                PutInTrash(id, index);
                counts[id] = index + 1;
            }

            AssertNumberOfCardsInTrash(lung, n);
        }
    }
}
