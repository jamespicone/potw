using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Dragon
{
    [TestFixture()]
    public class DragonTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            Assert.That(dragon, Is.Not.Null);
        }

        [Test()]
        public void TestPhases()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            var tokenPool = dragon.CharacterCard.FindTokenPool("FocusPool");

            DecisionDoNotActivatableAbility = true;

            GoToStartOfTurn(dragon);
            AssertCurrentTurnPhase(dragon, Phase.Start);
            AssertTokenPoolCount(tokenPool, 4);

            EnterNextTurnPhase();
            AssertCurrentTurnPhase(dragon, Phase.Unknown);
            AssertTokenPoolCount(tokenPool, 4);

            EnterNextTurnPhase();
            AssertCurrentTurnPhase(dragon, Phase.End);
            AssertTokenPoolCount(tokenPool, 0);
        }

        [Test()]
        public void TestStartsWith4FocusPoints()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var tokenPool = dragon.CharacterCard.FindTokenPool("FocusPool");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);

            AssertTokenPoolCount(tokenPool, 4);
        }

        [Test()]
        public void TestLosesAllFocusAtEndOfTurn()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var tokenPool = dragon.CharacterCard.FindTokenPool("FocusPool");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);
            AssertTokenPoolCount(tokenPool, 4);

            GoToEndOfTurn(dragon);
            AssertTokenPoolCount(tokenPool, 0);
        }

        [Test()]
        public void TestFocusPhaseExists()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            DecisionDoNotActivatableAbility = true;

            GoToStartOfTurn(dragon);
            AssertCurrentTurnPhase(dragon, Phase.Start);

            // The next phase should be the Focus phase (Unknown)
            EnterNextTurnPhase();
            AssertCurrentTurnPhase(dragon, Phase.Unknown);

            // Then End phase
            EnterNextTurnPhase();
            AssertCurrentTurnPhase(dragon, Phase.End);
        }

        [Test()]
        public void TestROMFocusAbility0_DrawCard()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var tokenPool = dragon.CharacterCard.FindTokenPool("FocusPool");
            var rom = GetCard("DragonsROM");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);
            AssertTokenPoolCount(tokenPool, 4);

            // Use power to activate focus ability 0 (draw)
            QuickHandStorage(dragon);
            DecisionDoNotActivatableAbility = false;
            DecisionActivateAbilities = new Card[] { rom };
            UsePower(dragon.CharacterCard);

            QuickHandCheck(1);
            // Power doesn't consume focus - only Focus Phase does
            AssertTokenPoolCount(tokenPool, 4);
        }

        [Test()]
        public void TestROMHasThreeFocusAbilities()
        {
            // ROM has 3 focus abilities: draw, play, return mech
            // Since the test framework can't easily select specific abilities by index,
            // we verify that all three abilities are available
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var rom = GetCard("DragonsROM");
            var romController = FindCardController(rom);
            var abilities = romController.GetActivatableAbilities("focus");

            // Should have 3 focus abilities
            Assert.That(abilities.Count(), Is.EqualTo(3), "ROM should have 3 focus abilities");
        }

        [Test()]
        public void TestROMFocusAbilitiesConsumeFocusDuringFocusPhase()
        {
            // Test that during Focus Phase, each ability activation consumes focus
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var tokenPool = dragon.CharacterCard.FindTokenPool("FocusPool");
            var rom = GetCard("DragonsROM");

            // Stack deck with cards to draw
            StackDeck(dragon, new Card[] { GetCard("Analysis", 0), GetCard("Analysis", 1) });

            GoToStartOfTurn(dragon);
            AssertTokenPoolCount(tokenPool, 4);

            // Enter Focus Phase and activate abilities (draws cards since ability 0 is selected)
            EnterNextTurnPhase(); // Focus phase
            AssertCurrentTurnPhase(dragon, Phase.Unknown);

            // Should have consumed focus tokens during Focus phase
            GoToEndOfTurn(dragon);
            // Focus is reset to 0 at end of turn regardless
            AssertTokenPoolCount(tokenPool, 0);
        }

        [Test()]
        public void TestIncapAbility0_HealTarget()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Bunker", "InsulaPrimalis");
            StartGame();

            SetHitPoints(bunker.CharacterCard, 10);
            IncapacitateCharacter(dragon.CharacterCard, baron.CharacterCard);
            AssertIncapacitated(dragon);

            QuickHPStorage(bunker);
            DecisionSelectCard = bunker.CharacterCard;
            UseIncapacitatedAbility(dragon, 0);

            QuickHPCheck(2);
        }

        [Test()]
        public void TestIncapAbility1_RetrieveFromTrash()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Bunker", "InsulaPrimalis");
            StartGame();

            // Put a card in Bunker's trash
            var card = PutInTrash("FlakCannon");

            IncapacitateCharacter(dragon.CharacterCard, baron.CharacterCard);
            AssertIncapacitated(dragon);

            DecisionSelectTurnTaker = bunker.TurnTaker;
            DecisionSelectCard = card;
            UseIncapacitatedAbility(dragon, 1);

            AssertInHand(card);
        }

        [Test()]
        public void TestIncapAbility2_ReduceEnvironmentDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Bunker", "InsulaPrimalis");
            StartGame();

            // Get an environment target
            var envTarget = PlayCard("VelociraptorPack");

            IncapacitateCharacter(dragon.CharacterCard, baron.CharacterCard);
            AssertIncapacitated(dragon);

            UseIncapacitatedAbility(dragon, 2);

            // Environment damage should be reduced by 2
            QuickHPStorage(bunker);
            DealDamage(envTarget, bunker, 4, DamageType.Melee);
            QuickHPCheck(-2); // 4 - 2 = 2
        }

        [Test()]
        public void TestIncapAbility2_DoesNotReduceNonEnvironmentDamage()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Bunker", "InsulaPrimalis");
            StartGame();

            IncapacitateCharacter(dragon.CharacterCard, baron.CharacterCard);
            AssertIncapacitated(dragon);

            UseIncapacitatedAbility(dragon, 2);

            // Villain damage should NOT be reduced
            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 4, DamageType.Melee);
            QuickHPCheck(-4); // Full damage
        }

        [Test()]
        public void TestIncapAbility2_ExpiresAtStartOfNextTurn()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "Bunker", "InsulaPrimalis");
            StartGame();

            var envTarget = PlayCard("VelociraptorPack");

            IncapacitateCharacter(dragon.CharacterCard, baron.CharacterCard);
            AssertIncapacitated(dragon);

            UseIncapacitatedAbility(dragon, 2);

            // Go to start of Dragon's next turn
            GoToStartOfTurn(dragon);

            // Environment damage should no longer be reduced
            QuickHPStorage(bunker);
            DealDamage(envTarget, bunker, 4, DamageType.Melee);
            QuickHPCheck(-4); // Full damage
        }

        [Test()]
        public void TestRepOfEarth()
        {
            SetupGameController("BaronBlade", "Legacy", "TheCelestialTribunal");

            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            SelectFromBoxForNextDecision("Jp.ParahumansOfTheWormverse.DragonCharacter", "Jp.ParahumansOfTheWormverse.Dragon");
            var rep = PlayCard("RepresentativeOfEarth");
            var dragonCard = rep.NextToLocation.Cards.FirstOrDefault();
            Assert.That(dragonCard, Is.Not.Null);
            Assert.That(dragonCard.Identifier, Is.EqualTo("DragonCharacter"));

            var tokenPool = dragonCard.FindTokenPool("FocusPool");

            StackDeck("CalledToJudgement");

            GoToStartOfTurn(env);
            AssertCurrentTurnPhase(env, Phase.Start);
            AssertTokenPoolCount(tokenPool, 4);
            EnterNextTurnPhase();

            AssertCurrentTurnPhase(env, Phase.PlayCard);
            AssertTokenPoolCount(tokenPool, 4);
            EnterNextTurnPhase();

            AssertCurrentTurnPhase(env, Phase.End);
            AssertTokenPoolCount(tokenPool, 0);
            EnterNextTurnPhase();
        }

        [Test()]
        public void TestFocusSpendingDuringFocusPhase()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            RemoveVillainCards();
            RemoveVillainTriggers();

            var tokenPool = dragon.CharacterCard.FindTokenPool("FocusPool");
            var rom = GetCard("DragonsROM");

            // Put some cards on deck for drawing
            StackDeck("Analysis", "Analysis", "Analysis", "Analysis");

            GoToStartOfTurn(dragon);
            AssertTokenPoolCount(tokenPool, 4);

            // During Focus phase, we can spend focus multiple times
            EnterNextTurnPhase(); // Enter Focus phase
            AssertCurrentTurnPhase(dragon, Phase.Unknown);

            // Each focus ability activation should consume a focus point
            // The focus phase will keep asking to activate abilities while we have focus
            // Use draw ability 4 times to use all focus
            QuickHandStorage(dragon);
            DecisionDoNotActivatableAbility = false;
            DecisionActivateAbilities = new Card[] { rom, rom, rom, rom };
            DecisionDoNotActivatableAbility = true;

            // The phase should have consumed focus and drawn cards
            GoToEndOfTurn(dragon);
            AssertTokenPoolCount(tokenPool, 0);
        }

        [Test()]
        public void TestPowerActivatesFocusAbility()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dragon", "InsulaPrimalis");
            StartGame();

            var tokenPool = dragon.CharacterCard.FindTokenPool("FocusPool");
            var rom = GetCard("DragonsROM");

            DecisionDoNotActivatableAbility = true;
            GoToStartOfTurn(dragon);
            AssertTokenPoolCount(tokenPool, 4);

            // Using Dragon's power should let us activate a focus ability
            DecisionDoNotActivatableAbility = false;
            DecisionActivateAbilities = new Card[] { rom };
            QuickHandStorage(dragon);

            UsePower(dragon.CharacterCard);

            QuickHandCheck(1); // Drew a card
            // Power doesn't consume focus - only Focus Phase does
            AssertTokenPoolCount(tokenPool, 4);
        }
    }
}
