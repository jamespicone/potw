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
                IEnumerator removeCoroutine = base.GameController.RemoveTokensFromPool(removingPool, numToPass, storedResults: removal, cardSource: GetCardSource());
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
                    IEnumerator addCoroutine = base.GameController.AddTokensToPool(addingPool, numRemoved, GetCardSource());
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
    }
}
