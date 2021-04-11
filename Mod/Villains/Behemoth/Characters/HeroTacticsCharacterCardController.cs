using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Behemoth
{
    public class HeroTacticsCharacterCardController : BehemothUtilityCharacterCardController
    {
        public HeroTacticsCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
            // Show all proximity pools?
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            // "This card is indestructible."
            if (card == base.Card)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void AddSideTriggers()
        {
            if (!base.Card.IsFlipped)
            {
                // Standard Protocols
                // "At the start of a player's turn, that player may skip the rest of their turn to remove 2 proximity tokens from their hero."
                AddStartOfTurnTrigger((TurnTaker tt) => tt.IsHero && !tt.IsIncapacitatedOrOutOfGame, SkipTurnOptionResponse, new TriggerType[] { TriggerType.SkipTurn, TriggerType.ModifyTokens });
                // "At the end of the environment turn, choose one:{BR}* Each player may move a proximity token from their hero to another active hero.{BR}* One player may move 2 proximity tokens from their hero to the active hero after them in the turn order.{BR}* One player may remove a proximity token from their hero."
                AddEndOfTurnTrigger((TurnTaker tt) => tt.IsEnvironment, StandardHeroMovementResponse, TriggerType.ModifyTokens);
            }
            else
            {
                // Panicked
                // "At the end of the environment turn, choose one:{BR}* Each player may move a proximity token from their hero to another active hero.{BR}* One player may move 2 proximity tokens from their hero to the active hero after them in the turn order."
                AddEndOfTurnTrigger((TurnTaker tt) => tt.IsEnvironment, PanickedHeroMovementResponse, TriggerType.ModifyTokens);
            }
            base.AddSideTriggers();
        }

        public IEnumerator SkipTurnOptionResponse(PhaseChangeAction pca)
        {
            // "... that player may skip the rest of their turn to remove 2 proximity tokens from their hero."
            List<Card> associated = FindCardsWhere((Card c) => c.Identifier == ProximityMarkerIdentifier && c.Location.HighestRecursiveLocation.OwnerTurnTaker == pca.ToPhase.TurnTaker).ToList();
            YesNoAmountDecision answer = new YesNoAmountDecision(base.GameController, base.GameController.FindTurnTakerController(pca.ToPhase.TurnTaker).ToHero(), SelectionType.SkipTurn, 1, associatedCards: associated, cardSource: GetCardSource());
            IEnumerator decideCoroutine = base.GameController.MakeDecisionAction(answer);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(decideCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(decideCoroutine);
            }
            if (DidPlayerAnswerYes(answer))
            {
                TokenPool proximityToReduce = ProximityPool(pca.ToPhase.TurnTaker);
                if (proximityToReduce != null)
                {
                    // Remove 2 tokens
                    IEnumerator removeCoroutine = base.GameController.RemoveTokensFromPool(proximityToReduce, 2, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(removeCoroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(removeCoroutine);
                    }
                }
                // Skip the rest of the turn
                IEnumerator skipCoroutine = base.GameController.SkipToNextTurn(GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(skipCoroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(skipCoroutine);
                }
            }
            yield break;
        }

        public IEnumerator StandardHeroMovementResponse(PhaseChangeAction pca)
        {
            // "... choose one:{BR}* Each player may move a proximity token from their hero to another active hero.{BR}* One player may move 2 proximity tokens from their hero to the active hero after them in the turn order.{BR}* One player may remove a proximity token from their hero."
            List<Function> options = new List<Function>();
            options.Add(new Function(DecisionMaker, "Each player may move 1 token to another active hero", SelectionType.RemoveTokens, EachPlayerMayMoveOneToken, onlyDisplayIfTrue: (bool?)!AllProximityPoolsEmpty()));
            options.Add(new Function(DecisionMaker, "One player may move 2 tokens to the next active hero in turn order", SelectionType.RemoveTokens, OnePlayerMayPassTwoTokens, onlyDisplayIfTrue: (bool?)!AllProximityPoolsEmpty()));
            options.Add(new Function(DecisionMaker, "One player may remove 1 token", SelectionType.RemoveTokens, OnePlayerMayRemoveOneToken, onlyDisplayIfTrue: (bool?)!AllProximityPoolsEmpty()));
            SelectFunctionDecision choice = new SelectFunctionDecision(base.GameController, DecisionMaker, options, false, noSelectableFunctionMessage: "There are no heroes with proximity tokens.", cardSource: GetCardSource());
            IEnumerator chooseCoroutine = base.GameController.SelectAndPerformFunction(choice);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(chooseCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(chooseCoroutine);
            }
            yield break;
        }

        public IEnumerator PanickedHeroMovementResponse(PhaseChangeAction pca)
        {
            // "... choose one:{BR}* Each player may move a proximity token from their hero to another active hero.{BR}* One player may move 2 proximity tokens from their hero to the active hero after them in the turn order."
            List<Function> options = new List<Function>();
            options.Add(new Function(DecisionMaker, "Each player may move 1 token to another active hero", SelectionType.RemoveTokens, EachPlayerMayMoveOneToken, onlyDisplayIfTrue: (bool?)!AllProximityPoolsEmpty()));
            options.Add(new Function(DecisionMaker, "One player may move 2 tokens to the next active hero in turn order", SelectionType.RemoveTokens, OnePlayerMayPassTwoTokens, onlyDisplayIfTrue: (bool?)!AllProximityPoolsEmpty()));
            SelectFunctionDecision choice = new SelectFunctionDecision(base.GameController, DecisionMaker, options, false, noSelectableFunctionMessage: "There are no heroes with proximity tokens.", cardSource: GetCardSource());
            IEnumerator chooseCoroutine = base.GameController.SelectAndPerformFunction(choice);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(chooseCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(chooseCoroutine);
            }
            yield break;
        }

        public IEnumerator EachPlayerMayMoveOneToken()
        {
            // "Each player may move a proximity token from their hero to another active hero."
            IEnumerator selectCoroutine = base.GameController.SelectTurnTakersAndDoAction(DecisionMaker, new LinqTurnTakerCriteria((TurnTaker tt) => tt.IsHero && !tt.IsIncapacitatedOrOutOfGame), SelectionType.RemoveTokens, MayMoveOneTokenResponse, requiredDecisions: 0, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(selectCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(selectCoroutine);
            }
            yield break;
        }

        public IEnumerator MayMoveOneTokenResponse(TurnTaker passingTT)
        {
            // If the selected player has a token, they may move it to another player's pool
            TokenPool passingPool = ProximityPool(passingTT);
            if (passingPool != null && passingPool.CurrentValue > 0)
            {
                List<SelectTurnTakerDecision> choice = new List<SelectTurnTakerDecision>();
                IEnumerator chooseCoroutine = base.GameController.SelectTurnTaker(base.GameController.FindHeroTurnTakerController(passingTT.ToHero()), SelectionType.AddTokens, choice, optional: true, additionalCriteria: (TurnTaker tt) => tt.IsHero && !tt.IsIncapacitatedOrOutOfGame && tt != passingTT, numberOfCards: 1, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(chooseCoroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(chooseCoroutine);
                }
                // If passingTT chose a recipient, move one of passingTT's tokens to the recipient
                if (DidSelectTurnTaker(choice))
                {
                    TurnTaker receivingTT = choice.FirstOrDefault().SelectedTurnTaker;
                    if (receivingTT != null)
                    {
                        TokenPool receivingPool = ProximityPool(receivingTT);
                        if (receivingPool != null)
                        {
                            IEnumerator removeCoroutine = base.GameController.RemoveTokensFromPool(passingPool, 1, cardSource: GetCardSource());
                            if (UseUnityCoroutines)
                            {
                                yield return GameController.StartCoroutine(removeCoroutine);
                            }
                            else
                            {
                                GameController.ExhaustCoroutine(removeCoroutine);
                            }
                            IEnumerator addCoroutine = base.GameController.AddTokensToPool(receivingPool, 1, GetCardSource());
                            if (UseUnityCoroutines)
                            {
                                yield return GameController.StartCoroutine(addCoroutine);
                            }
                            else
                            {
                                GameController.ExhaustCoroutine(addCoroutine);
                            }
                        }
                    }
                }
            }
            yield break;
        }

        public IEnumerator OnePlayerMayPassTwoTokens()
        {
            // "One player may move 2 proximity tokens from their hero to the active hero after them in the turn order."
            List<SelectTurnTakerDecision> choice = new List<SelectTurnTakerDecision>();
            IEnumerator selectCoroutine = base.GameController.SelectTurnTaker(DecisionMaker, SelectionType.RemoveTokens, choice, additionalCriteria: (TurnTaker tt) => ProximityPool(tt) != null && ProximityPool(tt).CurrentValue > 0, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(selectCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(selectCoroutine);
            }
            if (DidSelectTurnTaker(choice))
            {
                TurnTaker passingTT = choice.FirstOrDefault().SelectedTurnTaker;
                if (passingTT != null)
                {
                    TokenPool passingProximity = ProximityPool(passingTT);
                    if (passingProximity != null && passingProximity.CurrentValue > 0)
                    {
                        // Find the next active hero in turn order
                        TurnTaker[] activeHeroTurnOrder = base.GameController.FindTurnTakersWhere((TurnTaker tt) => tt.IsHero && !tt.IsIncapacitatedOrOutOfGame).ToArray();
                        int passingIndex = Array.IndexOf(activeHeroTurnOrder, passingTT);
                        int receivingIndex = -1;
                        if (passingIndex == activeHeroTurnOrder.Length - 1)
                        {
                            receivingIndex = 0;
                        }
                        else
                        {
                            receivingIndex = passingIndex + 1;
                        }
                        TurnTaker receivingTT = activeHeroTurnOrder.ElementAt(receivingIndex);
                        if (receivingTT != null)
                        {
                            TokenPool receivingProximity = ProximityPool(receivingTT);
                            if (receivingProximity != null)
                            {
                                List<RemoveTokensFromPoolAction> removeResults = new List<RemoveTokensFromPoolAction>();
                                IEnumerator removeCoroutine = base.GameController.RemoveTokensFromPool(passingProximity, 2, storedResults: removeResults, cardSource: GetCardSource());
                                if (UseUnityCoroutines)
                                {
                                    yield return GameController.StartCoroutine(removeCoroutine);
                                }
                                else
                                {
                                    GameController.ExhaustCoroutine(removeCoroutine);
                                }
                                int numRemoved = GetNumberOfTokensRemoved(removeResults);
                                IEnumerator addCoroutine = base.GameController.AddTokensToPool(receivingProximity, numRemoved, GetCardSource());
                                if (UseUnityCoroutines)
                                {
                                    yield return GameController.StartCoroutine(addCoroutine);
                                }
                                else
                                {
                                    GameController.ExhaustCoroutine(addCoroutine);
                                }
                            }
                        }
                    }
                }
            }
            yield break;
        }

        public IEnumerator OnePlayerMayRemoveOneToken()
        {
            // "One player may remove a proximity token from their hero."
            List<SelectTurnTakerDecision> choice = new List<SelectTurnTakerDecision>();
            IEnumerator selectCoroutine = base.GameController.SelectTurnTaker(DecisionMaker, SelectionType.RemoveTokens, choice, additionalCriteria: (TurnTaker tt) => ProximityPool(tt) != null && ProximityPool(tt).CurrentValue > 0, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(selectCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(selectCoroutine);
            }
            if (DidSelectTurnTaker(choice))
            {
                TurnTaker selected = choice.FirstOrDefault().SelectedTurnTaker;
                if (selected != null)
                {
                    TokenPool selectedProximity = ProximityPool(selected);
                    if (selectedProximity != null && selectedProximity.CurrentValue > 0)
                    {
                        IEnumerator removeCoroutine = base.GameController.RemoveTokensFromPool(selectedProximity, 1, cardSource: GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(removeCoroutine);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(removeCoroutine);
                        }
                    }
                }
            }
            yield break;
        }
    }
}
