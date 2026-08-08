using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Coil
{
    public abstract class CoilTestBase : ParahumanTest
    {
        protected Card scheming { get { return GetCard("CoilSchemingCharacter"); } }
        protected Card acting { get { return GetCard("CoilActingCharacter"); } }

        protected void SetupCoilGame(bool advanced = false)
        {
            SetupGameController(
                new string[] { "Jp.ParahumansOfTheWormverse.Coil", "Legacy", "Bunker", "Haka", "Megalopolis" },
                advanced: advanced);
            StartGame();
        }

        // The start of villain turn 1 fires Acting's reveal (which can put a random
        // parahuman into play) and Scheming's magic text (which plays the top card of
        // the environment deck). Clear that noise; Underground Base stays.
        protected void CleanupSetupNoise()
        {
            DestroyCards(c => c.IsEnvironment && c.IsInPlay);
            DestroyCards(c => c.IsVillain && !c.IsCharacter && c.IsInPlay && c.Identifier != "UndergroundBase");
        }

        // Remove the character side triggers so cards can be tested in isolation.
        protected void RemoveCoilTriggers()
        {
            RemoveCardTriggers(coil.CharacterCard);
            RemoveCardTriggers(scheming);
            RemoveCardTriggers(acting);
        }
    }
}
