using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra;

using Jp.ParahumansOfTheWormverse.Echidna;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Echidna
{
    [TestFixture()]
    public class EngulfedTests : ParahumanTest
    {
        [Test()]
        public void TestPlayLocation()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "Jp.ParahumansOfTheWormverse.Legend",
                "Megalopolis"
            );

            StackDeck("ApocryphaTwisted", "SpearpointTwisted", "ConvictionTwisted");

            RemoveAllTwisted();
            StartGame();            

            AssertNextDecisionChoices(
                new Card[] { alexandria.CharacterCard, bitch.CharacterCard, legend.CharacterCard },
                new Card[] { echidna.CharacterCard });
            AssertNextDecisionSelectionType(SelectionType.MoveCardNextToCard);

            DecisionSelectCard = bitch.CharacterCard;
            PlayCard("Engulfed", 0);
            ResetDecisions();

            AssertNextDecisionChoices(
                new Card[] { alexandria.CharacterCard, legend.CharacterCard },
                new Card[] { echidna.CharacterCard, bitch.CharacterCard });
            AssertNextDecisionSelectionType(SelectionType.MoveCardNextToCard);
            PlayCard("Engulfed", 1);
        }

        [Test()]
        public void TestHurtsOnPowers()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "Jp.ParahumansOfTheWormverse.Legend",
                "Megalopolis"
            );

            RemoveAllTwisted();
            StartGame();

            DecisionSelectCard = bitch.CharacterCard;
            PlayCard("Engulfed", 0);
            ResetDecisions();

            QuickHPStorage(bitch.CharacterCard, alexandria.CharacterCard, legend.CharacterCard);
            
            UsePower("BitchCharacter");
            QuickHPCheck(-2, 0, 0);

            UsePower("LegendCharacter");
            QuickHPCheck(0, 0, 0);
        }

        [Test()]
        public void TestPlaysTwisted()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Megalopolis"
            );

            StackDeck("PandemicTwisted");
            StackDeck("HubrisTwisted");

            StartGame();
            ReturnAllTwisted();

            StackDeck("PandemicTwisted");
            StackDeck("HubrisTwisted");

            AssertNotInPlay("PandemicTwisted");
            AssertNotInPlay("HubrisTwisted");

            PlayCard("Engulfed", 0);

            AssertNotInPlay("PandemicTwisted");
            AssertIsInPlay("HubrisTwisted");

            PlayCard("Engulfed", 1);

            // Nothing for card to play next to; doesn't play a twisted
            AssertNotInPlay("PandemicTwisted");
            AssertIsInPlay("HubrisTwisted");
        }
    }
}
