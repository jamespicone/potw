using Handelabra;
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
            base.AddSideTriggers();
            if (!base.Card.IsFlipped)
            {
                // Standard Protocols
                // "At the start of a player's turn, that player may skip the rest of their turn to remove 2 proximity tokens from their hero."
                base.AddSideTrigger(AddStartOfTurnTrigger((TurnTaker tt) => tt.IsHero && !tt.IsIncapacitatedOrOutOfGame, SkipTurnOptionResponse, new TriggerType[] { TriggerType.SkipTurn, TriggerType.ModifyTokens }));
                // "At the end of the environment turn, choose one:{BR}* Each player may move a proximity token from their hero to another active hero.{BR}* One player may move 2 proximity tokens from their hero to the active hero after them in the turn order.{BR}* One player may remove a proximity token from their hero."
                base.AddSideTrigger(AddEndOfTurnTrigger((TurnTaker tt) => tt.IsEnvironment, StandardHeroMovementResponse, TriggerType.ModifyTokens));
            }
            else
            {
                // Panicked
                // "At the end of the environment turn, choose one:{BR}* Each player may move a proximity token from their hero to another active hero.{BR}* One player may move 2 proximity tokens from their hero to the active hero after them in the turn order."
                base.AddSideTrigger(AddEndOfTurnTrigger((TurnTaker tt) => tt.IsEnvironment, PanickedHeroMovementResponse, TriggerType.ModifyTokens));
            }
        }

        public IEnumerator SkipTurnOptionResponse(PhaseChangeAction pca)
        {
            // "... that player may skip the rest of their turn to remove 2 proximity tokens from their hero."
            List<Card> associated = base.GameController.FindCardsWhere((Card c) => c.Identifier == ProximityMarkerIdentifier && c.Location.HighestRecursiveLocation.OwnerTurnTaker == pca.ToPhase.TurnTaker, realCardsOnly: false).ToList();
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
                    IEnumerator removeCoroutine = RemoveProximityTokens(pca.ToPhase.TurnTaker, 2, GetCardSource());
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
            IEnumerator selectCoroutine = base.GameController.SelectTurnTakersAndDoAction(DecisionMaker, new LinqTurnTakerCriteria((TurnTaker tt) => tt.IsHero && !tt.IsIncapacitatedOrOutOfGame), SelectionType.RemoveTokens, MayMoveOneTokenResponse, requiredDecisions: 0, allowAutoDecide: true, numberOfCards: 1, cardSource: GetCardSource());
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
                            IEnumerator passCoroutine = PassProximityTokens(passingTT, receivingTT, 1);
                            if (UseUnityCoroutines)
                            {
                                yield return GameController.StartCoroutine(passCoroutine);
                            }
                            else
                            {
                                GameController.ExhaustCoroutine(passCoroutine);
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
            IEnumerator selectCoroutine = base.GameController.SelectTurnTaker(DecisionMaker, SelectionType.RemoveTokens, choice, optional: true, additionalCriteria: (TurnTaker tt) => ProximityPool(tt) != null && ProximityPool(tt).CurrentValue > 0, cardSource: GetCardSource());
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
                                IEnumerator passCoroutine = PassProximityTokens(passingTT, receivingTT, 2);
                                if (UseUnityCoroutines)
                                {
                                    yield return GameController.StartCoroutine(passCoroutine);
                                }
                                else
                                {
                                    GameController.ExhaustCoroutine(passCoroutine);
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
            IEnumerator selectCoroutine = base.GameController.SelectTurnTaker(DecisionMaker, SelectionType.RemoveTokens, choice, optional: true, additionalCriteria: (TurnTaker tt) => ProximityPool(tt) != null && ProximityPool(tt).CurrentValue > 0, cardSource: GetCardSource());
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
                        IEnumerator removeCoroutine = RemoveProximityTokens(selected, 1, GetCardSource(), true);
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

        public IEnumerator PassProximityTokens(TurnTaker removingTT, TurnTaker addingTT, int numToPass)
        {
            // removingTT removes [numToPass] tokens, addingTT adds as many tokens as were removed
            TokenPool removingPool = ProximityPool(removingTT);
            TokenPool addingPool = ProximityPool(addingTT);
            if (removingPool != null && addingPool != null && removingPool.CurrentValue > 0 && numToPass > 0)
            {
                List<RemoveTokensFromPoolAction> removal = new List<RemoveTokensFromPoolAction>();
                IEnumerator removeCoroutine = RemoveProximityTokens(removingTT, numToPass, GetCardSource(), true, removal);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(removeCoroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(removeCoroutine);
                }
                if (DidRemoveTokens(removal))
                {
                    int numRemoved = GetNumberOfTokensRemoved(removal);
                    IEnumerator addCoroutine = AddProximityTokens(addingTT, numRemoved, GetCardSource(), true);
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
            yield break;
        }

        public IEnumerator AddProximityTokens(TurnTaker tt, int numTokens, CardSource cardSource = null, bool showUpdatedValue = false)
        {
            if (tt == null || !tt.IsHero || tt.IsIncapacitatedOrOutOfGame)
            {
                yield break;
            }
            // Add [numTokens] tokens to tt's proximity pool, accompanied by announcement message
            IEnumerator addCoroutine = base.GameController.AddTokensToPool(ProximityPool(tt), numTokens, cardSource);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(addCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(addCoroutine);
            }
            int numAdded = numTokens;
            if (numAdded > 0)
            {
                string message = "";
                if (cardSource == null || cardSource.Card == null)
                {
                    if (numAdded == 1)
                    {
                        message = numAdded.ToString() + " token was added to ";
                    }
                    else
                    {
                        message = numAdded.ToString() + " tokens were added to ";
                    }
                }
                else
                {
                    if (numAdded == 1)
                    {
                        message = cardSource.Card.Title + " added " + numAdded.ToString() + " token to ";
                    }
                    else
                    {
                        message = cardSource.Card.Title + " added " + numAdded.ToString() + " tokens to ";
                    }
                }
                if (message != "")
                {
                    if (tt.IsMultiCharacterTurnTaker && tt.NameRespectingVariant.EndsWith("s"))
                    {
                        message = message + tt.NameRespectingVariant + "' proximity pool";
                    }
                    else
                    {
                        message = message + tt.NameRespectingVariant + "'s proximity pool";
                    }
                    if (showUpdatedValue && ProximityPool(tt) != null)
                    {
                        message = message + ", making a total of " + ProximityPool(tt).CurrentValue.ToString() + ".";
                    }
                    else
                    {
                        message = message + ".";
                    }
                    Log.Debug(message);
                    IEnumerator announceCoroutine = base.GameController.SendMessageAction(message, Priority.Medium, cardSource);
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(announceCoroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(announceCoroutine);
                    }
                }
            }
            yield break;
        }

        public IEnumerator RemoveProximityTokens(TurnTaker tt, int numTokens, CardSource cardSource = null, bool showUpdatedValue = false, List<RemoveTokensFromPoolAction> storedResults = null)
        {
            if (tt == null || !tt.IsHero || tt.IsIncapacitatedOrOutOfGame)
            {
                yield break;
            }
            // Remove [numTokens] tokens from tt's proximity pool, accompanied by announcement message
            string message = "";
            if (ProximityPool(tt).CurrentValue <= 0)
            {
                if (tt.IsMultiCharacterTurnTaker && tt.NameRespectingVariant.EndsWith("s"))
                {
                    message = "There are no tokens in " + tt.NameRespectingVariant + "' proximity pool";
                }
                else
                {
                    message = "There are no tokens in " + tt.NameRespectingVariant + "'s proximity pool";
                }
                if (cardSource == null || cardSource.Card == null)
                {
                    message = message + " to remove.";
                }
                else
                {
                    message = message + " for " + cardSource.Card.Title + " to remove.";
                }
            }
            else
            {
                List<RemoveTokensFromPoolAction> results = new List<RemoveTokensFromPoolAction>();
                IEnumerator removeCoroutine = base.GameController.RemoveTokensFromPool(ProximityPool(tt), numTokens, storedResults: results, cardSource: cardSource);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(removeCoroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(removeCoroutine);
                }
                if (storedResults != null)
                {
                    storedResults.AddRange(results);
                }
                int numRemoved = GetNumberOfTokensRemoved(results);
                if (numRemoved > 0)
                {
                    if (cardSource == null || cardSource.Card == null)
                    {
                        if (numRemoved == 1)
                        {
                            message = numRemoved.ToString() + " token was removed from ";
                        }
                        else
                        {
                            message = numRemoved.ToString() + " tokens were removed from ";
                        }
                    }
                    else
                    {
                        if (numRemoved == 1)
                        {
                            message = cardSource.Card.Title + " removed " + numRemoved.ToString() + " token from ";
                        }
                        else
                        {
                            message = cardSource.Card.Title + " removed " + numRemoved.ToString() + " tokens from ";

                        }
                    }
                }
                if (message != "")
                {
                    if (tt.IsMultiCharacterTurnTaker && tt.NameRespectingVariant.EndsWith("s"))
                    {
                        message = message + tt.NameRespectingVariant + "' proximity pool";
                    }
                    else
                    {
                        message = message + tt.NameRespectingVariant + "'s proximity pool";
                    }
                    if (showUpdatedValue && ProximityPool(tt) != null)
                    {
                        message = message + ", leaving " + ProximityPool(tt).CurrentValue.ToString() + ".";
                    }
                    else
                    {
                        message = message + ".";
                    }
                }
            }
            if (message != "")
            {
                Log.Debug(message);
                IEnumerator announceCoroutine = base.GameController.SendMessageAction(message, Priority.Medium, cardSource);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(announceCoroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(announceCoroutine);
                }
            }
            yield break;
        }
    }
}
