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
    public class BehemothUtilityCardController : CardController
    {
        public BehemothUtilityCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public const string ProximityMarkerIdentifier = "Proximity";
        public const string ProximityPoolIdentifier = "ProximityPool";

        public const string MovementDeckIdentifier = "MovementDeck";
        public const string HeroTacticsIdentifier = "HeroTacticsCharacter";

        public TokenPool ProximityPool(TurnTaker tt)
        {
            if (tt.IsHero && !tt.IsIncapacitatedOrOutOfGame)
            {
                Card proximityMarker = base.GameController.FindCardsWhere(new LinqCardCriteria((Card c) => c.Identifier == ProximityMarkerIdentifier && c.Location == tt.PlayArea), realCardsOnly: false).FirstOrDefault();
                if (proximityMarker != null)
                {
                    TokenPool proximityPool = proximityMarker.FindTokenPool(ProximityPoolIdentifier);
                    return proximityPool;
                }
            }
            return null;
        }

        public TurnTaker TurnTakerForPool(TokenPool proximityPool)
        {
            if (proximityPool.Identifier == ProximityPoolIdentifier)
            {
                Card proximityMarker = proximityPool.CardWithTokenPool;
                if (proximityMarker.Identifier == ProximityMarkerIdentifier && proximityMarker.IsInPlayAndHasGameText)
                {
                    return proximityMarker.Location.HighestRecursiveLocation.OwnerTurnTaker;
                }
            }
            return null;
        }

        public int? ProximityTokens(Card c)
        {
            if (c.IsHero)
            {
                TurnTaker player = c.Owner;
                TokenPool proximity = ProximityPool(player);
                if (proximity != null)
                {
                    return proximity.CurrentValue;
                }
            }
            return 0;
        }

        public bool AllProximityPoolsEmpty()
        {
            IEnumerable<HeroTurnTakerController> heroControllers = base.GameController.FindHeroTurnTakerControllers();
            bool allEmpty = true;
            foreach (HeroTurnTakerController player in heroControllers)
            {
                if (!player.IsIncapacitatedOrOutOfGame)
                {
                    TokenPool playerProximity = ProximityPool(player.TurnTaker);
                    if (playerProximity != null)
                    {
                        allEmpty &= playerProximity.CurrentValue == 0;
                    }
                }
            }
            return allEmpty;
        }

        public IOrderedEnumerable<TokenPool> OrderPoolsByHighestValue()
        {
            List<TokenPool> proximities = new List<TokenPool>();
            IEnumerable<HeroTurnTakerController> heroControllers = base.GameController.FindHeroTurnTakerControllers().Where((HeroTurnTakerController httc) => !httc.IsIncapacitatedOrOutOfGame);
            foreach(HeroTurnTakerController player in heroControllers)
            {
                TokenPool playerProximity = ProximityPool(player.TurnTaker);
                if (playerProximity != null)
                {
                    proximities.Add(playerProximity);
                }
            }
            return proximities.OrderByDescending((TokenPool tp) => tp.CurrentValue);
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

        public IEnumerator TakeProximityTokens(TurnTaker receivingTT, int numTokens)
        {
            // receivingTT gets [numTokens] tokens from another hero, but the group chooses which one
            IEnumerable<TurnTaker> passOptions = FindTurnTakersWhere((TurnTaker tt) => tt.IsHero && tt != receivingTT && !tt.IsIncapacitatedOrOutOfGame && ProximityPool(tt) != null && ProximityPool(tt).CurrentValue > 0);
            if (passOptions.Count() > 0)
            {
                TurnTaker passingTT = null;
                if (passOptions.Count() == 1)
                {
                    passingTT = passOptions.FirstOrDefault();
                }
                else
                {
                    List<SelectTurnTakerDecision> passChoice = new List<SelectTurnTakerDecision>();
                    IEnumerator selectCoroutine = base.GameController.SelectTurnTaker(DecisionMaker, SelectionType.RemoveTokens, passChoice, additionalCriteria: (TurnTaker tt) => passOptions.Contains(tt), numberOfCards: numTokens, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(selectCoroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(selectCoroutine);
                    }
                    if (DidSelectTurnTaker(passChoice))
                    {
                        passingTT = passChoice.FirstOrDefault().SelectedTurnTaker;
                    }
                }
                if (passingTT != null)
                {
                    IEnumerator passCoroutine = PassProximityTokens(passingTT, receivingTT, numTokens);
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
            yield break;
        }

        public IEnumerator SetBehemothDamageType(Card responsibleCard, DamageType newType)
        {
            BehemothCharacterCardController behemoth = (FindCardController(base.CharacterCard) as BehemothCharacterCardController);
            IEnumerator setCoroutine = behemoth.SetDamageType(responsibleCard, newType);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(setCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(setCoroutine);
            }
            yield break;
        }

        public DamageType GetBehemothDamageType()
        {
            BehemothCharacterCardController behemoth = (FindCardController(base.CharacterCard) as BehemothCharacterCardController);
            return behemoth.CurrentDamageType();
        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {
            return new CustomDecisionText("Do you want to move a token to another hero?", "Should they move a token to another hero?", "Vote for whether they should move a token to another hero", "moving a token to another hero", true);
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
