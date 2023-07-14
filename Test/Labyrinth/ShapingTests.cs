using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Jp.ParahumansOfTheWormverse.Labyrinth;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.GreazerTeam;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Labyrinth
{
    [TestFixture()]
    public class ShapingTests : BaseTest
    {
        protected HeroTurnTakerController labyrinth { get { return FindHero("Labyrinth"); } }
        protected HeroTurnTakerController tattletale { get { return FindHero("Tattletale"); } }

        [Test()]
        public void TestNeedsEnvironmentBase()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "InsulaPrimalis");

            StartGame();

            var card = PlayCard("BeautifulGarden");
            AssertNotInPlay(card);
            AssertInTrash(card);
        }

        [Test()]
        public void TestPlayedOntoEnvCard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "InsulaPrimalis");

            StartGame();

            AssertNextDecisionSelectionType(SelectionType.MoveCardAboveCard);
            var field = PlayCard("ObsidianField");
            var card = PlayCard("BeautifulGarden");
            AssertIsInPlay(card);
            AssertUnderCard(card, field);

            // Check that the field isn't still doing its thing
            QuickHPStorage(labyrinth);
            DealDamage(labyrinth, labyrinth, 1, DamageType.Melee);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestDestroyedWhenNoCardUnderIt()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "InsulaPrimalis");

            StartGame();

            AssertNextDecisionSelectionType(SelectionType.MoveCardAboveCard);
            var field = PlayCard("ObsidianField");
            var card = PlayCard("BeautifulGarden");
            AssertIsInPlay(card);
            AssertUnderCard(card, field);

            DestroyCard(field);

            AssertInTrash(card);
        }

        [Test()]
        public void TestCardsGoToFieldWhenShapingDestroyed()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "InsulaPrimalis");

            StartGame();

            AssertNextDecisionSelectionType(SelectionType.MoveCardAboveCard);
            var field = PlayCard("ObsidianField");
            var card = PlayCard("BeautifulGarden");
            AssertIsInPlay(card);
            AssertUnderCard(card, field);

            DestroyCard(card);

            AssertInTrash(card);
            AssertIsInPlay(field);

            // Check that the field is still doing its thing
            QuickHPStorage(labyrinth);
            DealDamage(labyrinth, labyrinth, 1, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestLabyrinthIncap()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Legacy", "InsulaPrimalis");

            StartGame();

            AssertNextDecisionSelectionType(SelectionType.MoveCardAboveCard);
            var field = PlayCard("ObsidianField");
            var card = PlayCard("BeautifulGarden");
            AssertIsInPlay(card);
            AssertUnderCard(card, field);

            DealDamage(labyrinth, labyrinth, 30, DamageType.Infernal);

            AssertIncapacitated(labyrinth);

            AssertOutOfGame(card);
            AssertIsInPlay(field);

            // Check that the field is still doing its thing
            QuickHPStorage(legacy);
            DealDamage(legacy, legacy, 1, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestLabyrinthIncapWithShapingOnShaping()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Legacy", "InsulaPrimalis");

            StartGame();

            AssertNextDecisionSelectionType(SelectionType.MoveCardAboveCard);
            var basecard = PlayCard("ObsidianField");
            var inbetween = PlayCard("BeautifulGarden");
            var topcard = PlayCard("MightyCastle");

            AssertIsInPlay(topcard);
            AssertUnderCard(inbetween, basecard);
            AssertUnderCard(topcard, inbetween);


            DealDamage(labyrinth, labyrinth, 30, DamageType.Infernal);

            AssertIncapacitated(labyrinth);

            AssertOutOfGame(inbetween);
            AssertOutOfGame(topcard);
            AssertIsInPlay(basecard);

            // Check that the field is still doing its thing
            QuickHPStorage(legacy);
            DealDamage(legacy, legacy, 1, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestShapingOnShaping()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Labyrinth", "Legacy", "InsulaPrimalis");

            StartGame();

            AssertNextDecisionSelectionType(SelectionType.MoveCardAboveCard);
            var basecard = PlayCard("ObsidianField");
            var inbetween = PlayCard("BeautifulGarden");
            var topcard = PlayCard("MightyCastle");

            DecisionSelectCard = inbetween;

            AssertIsInPlay(topcard);
            AssertUnderCard(inbetween, basecard);
            AssertUnderCard(topcard, inbetween);
        }
    }
}
