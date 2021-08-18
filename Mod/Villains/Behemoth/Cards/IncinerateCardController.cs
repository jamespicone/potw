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
    public class IncinerateCardController : BehemothUtilityCardController
    {
        public IncinerateCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            // "Change {BehemothCharacter}'s damage type to fire."
            IEnumerator fireCoroutine = SetBehemothDamageType(base.Card, DamageType.Fire);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(fireCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(fireCoroutine);
            }
            // "Destroy 2 hero Ongoing cards."
            List<DestroyCardAction> results = new List<DestroyCardAction>();
            IEnumerator destroyCoroutine = base.GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => c.IsHero && c.DoKeywordsContain("ongoing"), "hero ongoing"), 2, storedResultsAction: results, responsibleCard: base.Card, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(destroyCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(destroyCoroutine);
            }
            // "Move 1 proximity token from 1 hero whose card was destroyed this way to another active hero."
            if (DidDestroyCard(results))
            {
                IEnumerable<TurnTaker> playersWhoDestroyed = (from DestroyCardAction dca in results select dca.CardToDestroy.Card.Owner).Distinct();
                IEnumerable<TurnTaker> destroyedAndCanPass = playersWhoDestroyed.Where((TurnTaker tt) => ProximityPool(tt).CurrentValue > 0);
                if (destroyedAndCanPass.Count() > 0)
                {
                    TurnTaker passingTT = null;
                    if (destroyedAndCanPass.Count() > 1)
                    {
                        // Let players choose who moves a token
                        List<SelectTurnTakerDecision> passChoice = new List<SelectTurnTakerDecision>();
                        IEnumerator selectCoroutine = base.GameController.SelectTurnTaker(DecisionMaker, SelectionType.RemoveTokens, passChoice, additionalCriteria: (TurnTaker tt) => destroyedAndCanPass.Contains(tt), cardSource: GetCardSource());
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
                    else
                    {
                        passingTT = destroyedAndCanPass.FirstOrDefault();
                    }
                    // passingTT gets to choose who to pass to
                    List<SelectTurnTakerDecision> receivingChoice = new List<SelectTurnTakerDecision>();
                    IEnumerator chooseCoroutine = base.GameController.SelectTurnTaker(base.GameController.FindHeroTurnTakerController(passingTT.ToHero()), SelectionType.AddTokens, receivingChoice, additionalCriteria: (TurnTaker tt) => tt.IsHero && !tt.IsIncapacitatedOrOutOfGame && tt != passingTT, numberOfCards: 1, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(chooseCoroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(chooseCoroutine);
                    }
                    // If passingTT chose a recipient, move one of passingTT's tokens to the recipient
                    if (DidSelectTurnTaker(receivingChoice))
                    {
                        TurnTaker receivingTT = receivingChoice.FirstOrDefault().SelectedTurnTaker;
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
            }
            yield break;
        }
    }
}
