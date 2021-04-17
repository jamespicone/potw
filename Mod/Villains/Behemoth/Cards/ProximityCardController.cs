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
    public class ProximityCardController : BehemothUtilityCardController
    {
        public ProximityCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
            SpecialStringMaker.ShowLocationOfCards(new LinqCardCriteria((Card c) => c == base.Card, base.Card.Title, useCardsSuffix: false), specifyPlayAreas: true).Condition = () => base.Card.IsInPlayAndHasGameText;
            SpecialStringMaker.ShowSpecialString(PoolDisplay, () => base.Card.IsInPlayAndHasGameText, RelevantCharacters);
        }

        public string PoolDisplay()
        {
            string summary = "";
            int numTokens = base.Card.FindTokenPool(ProximityPoolIdentifier).CurrentValue;
            TurnTaker tt = base.Card.Location.HighestRecursiveLocation.OwnerTurnTaker;
            if (tt != null && tt.IsHero && !tt.IsIncapacitatedOrOutOfGame)
            {
                string ttName = tt.NameRespectingVariant;
                if (numTokens == 1)
                {
                    summary = "There is " + numTokens.ToString() + " token in " + ttName + "'s proximity pool.";
                }
                else
                {
                    summary = "There are " + numTokens.ToString() + " tokens in " + ttName + "'s proximity pool.";
                }
            }
            else
            {
                if (numTokens == 1)
                {
                    summary = "There is " + numTokens.ToString() + " token in this unassigned proximity pool.";
                }
                else
                {
                    summary = "There are " + numTokens.ToString() + " tokens in this unassigned proximity pool.";
                }
            }
            return summary;
        }

        public IEnumerable<Card> RelevantCharacters()
        {
            IEnumerable<Card> relevant = base.GameController.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsHeroCharacterCard && c.Owner == base.Card.Location.HighestRecursiveLocation.OwnerTurnTaker), false);
            return relevant;
        }

        public override void AddTriggers()
        {
            // "When you have no non-incapacitated heroes, remove all tokens from this card and remove this card from the game."
            AddTrigger<TargetLeavesPlayAction>((TargetLeavesPlayAction tlpa) => tlpa.TargetLeavingPlay.Owner == AssociatedTurnTaker() && AssociatedTurnTaker().IsIncapacitatedOrOutOfGame, RemoveResponse, new TriggerType[] { TriggerType.ModifyTokens, TriggerType.RemoveFromGame }, TriggerTiming.After);
            AddTrigger<GameAction>((GameAction ga) => AssociatedTurnTaker().IsIncapacitatedOrOutOfGame, RemoveResponse, new TriggerType[] { TriggerType.ModifyTokens, TriggerType.RemoveFromGame }, TriggerTiming.After);
            base.AddTriggers();
        }

        public TurnTaker AssociatedTurnTaker()
        {
            return base.Card.Location.HighestRecursiveLocation.OwnerTurnTaker;
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            // "This card exists to mark your hero's proximity token pool. It's indestructible as long as you have an active hero."
            if (card == base.Card)
            {
                return AssociatedTurnTaker().IsHero && !AssociatedTurnTaker().IsIncapacitatedOrOutOfGame;
            }
            return base.AskIfCardIsIndestructible(card);
        }

        public override IEnumerator Play()
        {
            // Set tokens to 0
            IEnumerator resetCoroutine = ResetTokenValue();
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(resetCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(resetCoroutine);
            }
            yield break;
        }

        public IEnumerator ResetTokenValue()
        {
            base.Card.FindTokenPool(ProximityPoolIdentifier).SetToInitialValue();
            yield return null;
        }

        public IEnumerator RemoveResponse(GameAction ga)
        {
            // "... remove all tokens from this card..."
            IEnumerator resetCoroutine = ResetTokenValue();
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(resetCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(resetCoroutine);
            }
            // "... and remove this card from the game."
            IEnumerator removeCoroutine = base.GameController.MoveCard(base.TurnTakerController, base.Card, base.TurnTaker.OutOfGame, responsibleTurnTaker: base.TurnTaker, actionSource: ga, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(removeCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(removeCoroutine);
            }
            yield break;
        }
    }
}
