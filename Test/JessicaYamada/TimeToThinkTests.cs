using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;
using Handelabra;

namespace Jp.ParahumansOfTheWormverse.UnitTest.JessicaYamada
{
    [TestFixture()]
    public class TimeToThinkTests : ParahumanTest
    {
        [Test()]
        public void TestNoPlayNoPower()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Tachyon", "Megalopolis");

            StartGame();
            RemoveVillainCards();
            RemoveVillainTriggers();

            GoToPlayCardPhaseAndPlayCard(jessica, "TimeToThink");

            GoToStartOfTurn(tachyon);
            QuickHandStorage(tachyon);
            EnterNextTurnPhase(); // enter play
            EnterNextTurnPhase(); // leave play / enter power
            QuickHandCheck(1);
            EnterNextTurnPhase(); // leave power
            QuickHandCheck(1);
        }

        [Test()]
        public void TestNoPlay()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Legacy", "Megalopolis");

            StartGame();
            RemoveVillainCards();
            RemoveVillainTriggers();

            GoToPlayCardPhaseAndPlayCard(jessica, "TimeToThink");

            GoToStartOfTurn(legacy);
            QuickHandStorage(legacy);
            EnterNextTurnPhase(); // enter play
            EnterNextTurnPhase(); // leave play / enter power
            QuickHandCheck(1);
            UsePower(legacy);
            EnterNextTurnPhase(); // leave power
            QuickHandCheck(0);
        }

        [Test()]
        public void TestNoPlayNoPowerRuleBreaking()
        {
            SetupGameController("WagerMaster", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Tachyon", "Megalopolis");

            RemoveVillainCards();
            RemoveVillainTriggers();

            MoveAllCards(wager, wager.TurnTaker.PlayArea, wager.TurnTaker.OutOfGame);

            StartGame();

            PlayCard("BreakingTheRules");

            GoToPlayCardPhaseAndPlayCard(jessica, "TimeToThink");

            GoToEndOfTurn(tachyon);
            QuickHandStorage(tachyon);
            EnterNextTurnPhase(); // enter draw phase
            EnterNextTurnPhase(); // enter power
            EnterNextTurnPhase(); // leave power/enter play
            QuickHandCheck(1); 
            EnterNextTurnPhase(); // leave play
            QuickHandCheck(1);
        }

        [Test()]
        public void TestNoPlayRuleBreakingNoCards()
        {
            SetupGameController("WagerMaster", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Legacy", "Megalopolis");

            RemoveVillainCards();
            RemoveVillainTriggers();

            MoveAllCards(wager, wager.TurnTaker.PlayArea, wager.TurnTaker.OutOfGame);

            StartGame();

            PlayCard("BreakingTheRules");

            GoToPlayCardPhaseAndPlayCard(jessica, "TimeToThink");

            MoveAllCardsFromHandToDeck(legacy);

            GoToEndOfTurn(legacy);
            QuickHandStorage(legacy);
            EnterNextTurnPhase(); // enter draw phase
            EnterNextTurnPhase(); // enter power
            UsePower(legacy);
            EnterNextTurnPhase(); // leave power/enter play
            QuickHandCheck(0);
            EnterNextTurnPhase(); // leave play
            QuickHandCheck(1);
        }

        [Test()]
        public void TestNoPlayNoPowerNoCardsNoPowers()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Tachyon", "Megalopolis");

            StartGame();
            RemoveVillainCards();
            RemoveVillainTriggers();

            GoToPlayCardPhaseAndPlayCard(jessica, "TimeToThink");

            MoveAllCardsFromHandToDeck(tachyon);

            var effect = new CannotUsePowersStatusEffect();
            effect.TurnTakerCriteria.IsHero = true;

            RunCoroutine(
                GameController.AddStatusEffect(
                    effect,
                    true,
                    new CardSource(tachyon.CharacterCardController)
                )
            );

            GoToStartOfTurn(tachyon);
            QuickHandStorage(tachyon);
            EnterNextTurnPhase(); // enter play
            EnterNextTurnPhase(); // leave play / enter power
            QuickHandCheck(1);
            AssertNumberOfUsablePowers(tachyon, 0);
            EnterNextTurnPhase(); // leave power
            QuickHandCheck(1);
        }

        [Test()]
        public void TestNoPlayNoCards()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "Legacy", "Megalopolis");

            StartGame();
            RemoveVillainCards();
            RemoveVillainTriggers();

            GoToPlayCardPhaseAndPlayCard(jessica, "TimeToThink");

            MoveAllCardsFromHandToDeck(legacy);

            GoToStartOfTurn(legacy);
            QuickHandStorage(legacy);
            EnterNextTurnPhase(); // enter play
            EnterNextTurnPhase(); // leave play / enter power
            QuickHandCheck(1);
            UsePower(legacy);
            EnterNextTurnPhase(); // leave power
            QuickHandCheck(0);
        }

        [Test()]
        public void TestNoPlayNoPowerConcordantHelm()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "LaComodora", "Megalopolis");

            RemoveVillainCards();
            RemoveVillainTriggers();

            StartGame();

            var effect = new CannotUsePowersStatusEffect();
            effect.TurnTakerCriteria.IsHero = true;

            RunCoroutine(
                GameController.AddStatusEffect(
                    effect,
                    true,
                    new CardSource(comodora.CharacterCardController)
                )
            );

            PlayCard("ConcordantHelm");

            GoToPlayCardPhaseAndPlayCard(jessica, "TimeToThink");

            var play = comodora.TurnTaker.TurnPhases.Where(tp => tp.Phase == Phase.PlayCard).First();
            var power = comodora.TurnTaker.TurnPhases.Where(tp => tp.Phase == Phase.UsePower).First();
            var draw = comodora.TurnTaker.TurnPhases.Where(tp => tp.Phase == Phase.DrawCard).First();

            // PUD
            GoToStartOfTurn(comodora);
            Log.Debug("PUD order");
            MoveAllCardsFromHandToDeck(comodora);
            QuickHandStorage(comodora);
            DecisionSelectTurnPhase = play; EnterNextTurnPhase();
            DecisionSelectTurnPhase = power; EnterNextTurnPhase();
            QuickHandCheck(1);
            DecisionSelectTurnPhase = draw; EnterNextTurnPhase();
            QuickHandCheck(1);

            // PDU
            GoToStartOfTurn(comodora);
            Log.Debug("PDU order");
            MoveAllCardsFromHandToDeck(comodora);
            QuickHandStorage(comodora);
            DecisionSelectTurnPhase = play; EnterNextTurnPhase();
            DecisionSelectTurnPhase = draw; EnterNextTurnPhase();
            QuickHandCheck(1);
            DecisionSelectTurnPhase = power; EnterNextTurnPhase();
            EnterNextTurnPhase();
            QuickHandCheck(1);

            // UPD
            GoToStartOfTurn(comodora);
            Log.Debug("UPD order");
            MoveAllCardsFromHandToDeck(comodora);
            QuickHandStorage(comodora);
            DecisionSelectTurnPhase = power; EnterNextTurnPhase();
            DecisionSelectTurnPhase = play; EnterNextTurnPhase();
            QuickHandCheck(1);
            DecisionSelectTurnPhase = draw; EnterNextTurnPhase();
            QuickHandCheck(1);

            // UDP
            GoToStartOfTurn(comodora);
            Log.Debug("UDP order");
            MoveAllCardsFromHandToDeck(comodora);
            QuickHandStorage(comodora);
            DecisionSelectTurnPhase = power; EnterNextTurnPhase();
            DecisionSelectTurnPhase = draw; EnterNextTurnPhase();
            QuickHandCheck(1);
            DecisionSelectTurnPhase = play; EnterNextTurnPhase();
            EnterNextTurnPhase();
            QuickHandCheck(1);

            // DUP
            GoToStartOfTurn(comodora);
            Log.Debug("DUP order");
            MoveAllCardsFromHandToDeck(comodora);
            QuickHandStorage(comodora);
            DecisionSelectTurnPhase = draw; EnterNextTurnPhase();
            DecisionSelectTurnPhase = power; EnterNextTurnPhase();
            DecisionSelectTurnPhase = play; EnterNextTurnPhase();
            QuickHandCheck(1);
            EnterNextTurnPhase();
            QuickHandCheck(1);

            // DPU
            GoToStartOfTurn(comodora);
            Log.Debug("DPU order");
            MoveAllCardsFromHandToDeck(comodora);
            QuickHandStorage(comodora);
            DecisionSelectTurnPhase = draw; EnterNextTurnPhase();
            DecisionSelectTurnPhase = play; EnterNextTurnPhase();
            DecisionSelectTurnPhase = power; EnterNextTurnPhase();
            QuickHandCheck(1);           
            EnterNextTurnPhase();
            QuickHandCheck(1);
        }

        [Test()]
        public void TestNoPlayConcordantHelmPowerUse()
        {
            SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.JessicaYamada", "LaComodora", "Megalopolis");

            RemoveVillainCards();
            RemoveVillainTriggers();

            StartGame();

            PlayCard("ConcordantHelm");

            GoToPlayCardPhaseAndPlayCard(jessica, "TimeToThink");

            var play = comodora.TurnTaker.TurnPhases.Where(tp => tp.Phase == Phase.PlayCard).First();
            var power = comodora.TurnTaker.TurnPhases.Where(tp => tp.Phase == Phase.UsePower).First();
            var draw = comodora.TurnTaker.TurnPhases.Where(tp => tp.Phase == Phase.DrawCard).First();

            // PUD
            GoToStartOfTurn(comodora);
            Log.Debug("PUD order");
            MoveAllCardsFromHandToDeck(comodora);
            QuickHandStorage(comodora);
            DecisionSelectTurnPhase = play; EnterNextTurnPhase();
            DecisionSelectTurnPhase = power; EnterNextTurnPhase();
            QuickHandCheck(1);
            UsePower(comodora);
            DecisionSelectTurnPhase = draw; EnterNextTurnPhase();
            QuickHandCheck(0);

            // PDU
            GoToStartOfTurn(comodora);
            Log.Debug("PDU order");
            MoveAllCardsFromHandToDeck(comodora);
            QuickHandStorage(comodora);
            DecisionSelectTurnPhase = play; EnterNextTurnPhase();
            DecisionSelectTurnPhase = draw; EnterNextTurnPhase();
            QuickHandCheck(1);
            DecisionSelectTurnPhase = power; EnterNextTurnPhase();
            UsePower(comodora);
            EnterNextTurnPhase();
            QuickHandCheck(0);

            // UPD
            GoToStartOfTurn(comodora);
            Log.Debug("UPD order");
            MoveAllCardsFromHandToDeck(comodora);
            QuickHandStorage(comodora);
            DecisionSelectTurnPhase = power; EnterNextTurnPhase();
            UsePower(comodora);
            DecisionSelectTurnPhase = play; EnterNextTurnPhase();
            QuickHandCheck(0);
            DecisionSelectTurnPhase = draw; EnterNextTurnPhase();
            QuickHandCheck(1);

            // UDP
            GoToStartOfTurn(comodora);
            Log.Debug("UDP order");
            MoveAllCardsFromHandToDeck(comodora);
            QuickHandStorage(comodora);
            DecisionSelectTurnPhase = power; EnterNextTurnPhase();
            UsePower(comodora);
            DecisionSelectTurnPhase = draw; EnterNextTurnPhase();
            QuickHandCheck(0);
            DecisionSelectTurnPhase = play; EnterNextTurnPhase();
            EnterNextTurnPhase();
            QuickHandCheck(1);

            // DUP
            GoToStartOfTurn(comodora);
            Log.Debug("DUP order");
            MoveAllCardsFromHandToDeck(comodora);
            QuickHandStorage(comodora);
            DecisionSelectTurnPhase = draw; EnterNextTurnPhase();
            DecisionSelectTurnPhase = power; EnterNextTurnPhase();
            UsePower(comodora);
            DecisionSelectTurnPhase = play; EnterNextTurnPhase();
            QuickHandCheck(0);
            EnterNextTurnPhase();
            QuickHandCheck(1);

            // DPU
            GoToStartOfTurn(comodora);
            Log.Debug("DPU order");
            MoveAllCardsFromHandToDeck(comodora);
            QuickHandStorage(comodora);
            DecisionSelectTurnPhase = draw; EnterNextTurnPhase();
            DecisionSelectTurnPhase = play; EnterNextTurnPhase();
            DecisionSelectTurnPhase = power; EnterNextTurnPhase();
            UsePower(comodora);
            QuickHandCheck(1);
            EnterNextTurnPhase();
            QuickHandCheck(0);
        }
    }
}
