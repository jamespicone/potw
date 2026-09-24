using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Skitter
{
    public class SkitterWeaverCharacterCardController : HeroCharacterCardController
    {
        private const string FirstStrategyThisRound = "FirstStrategyThisRound";

        public SkitterWeaverCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowIfElseSpecialString(
                () => HasBeenSetToTrueThisRound(FirstStrategyThisRound),
                () => $"One of {TurnTaker.Name}'s Strategy cards has entered play this round.",
                () => $"None of {TurnTaker.Name}'s Strategy cards have entered play this round."
            ).Condition = () => !Card.IsFlipped;
        }

        public override void AddSideTriggers()
        {
            if (!Card.IsFlipped)
            {
                // "The first time one of your Strategy cards enters play each round, another player may play a card."
                AddSideTrigger(AddTrigger<CardEntersPlayAction>(
                    cep => cep.CardEnteringPlay.Owner == TurnTaker &&
                        cep.CardEnteringPlay.Is().WithKeyword("strategy").AccordingTo(this) &&
                        !HasBeenSetToTrueThisRound(FirstStrategyThisRound),
                    FirstStrategyResponse,
                    TriggerType.PlayCard,
                    TriggerTiming.After
                ));
            }
        }

        private IEnumerator FirstStrategyResponse(CardEntersPlayAction cep)
        {
            SetCardPropertyToTrueIfRealAction(FirstStrategyThisRound);

            var e = GameController.SelectHeroToPlayCard(
                DecisionMaker,
                optionalSelectHero: false,
                optionalPlayCard: true,
                additionalCriteria: new LinqTurnTakerCriteria(tt => tt != TurnTaker, "other players"),
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // "Either another player draws a card or place a Bug token on up to 2 Strategy cards."
            var otherPlayers = new LinqTurnTakerCriteria(tt => tt != TurnTaker && tt.IsPlayer && !tt.IsIncapacitatedOrOutOfGame, "other players");
            Func<Card, bool> strategyWithPool = c => c.IsInPlayAndHasGameText && c.Is().WithKeyword("strategy").AccordingTo(this) && c.FindBugPool() != null;

            var functions = new List<Function>
            {
                new Function(
                    DecisionMaker,
                    "Another player draws a card",
                    SelectionType.DrawCard,
                    () => GameController.SelectHeroToDrawCard(DecisionMaker, optionalDrawCard: false, additionalCriteria: otherPlayers, cardSource: GetCardSource()),
                    onlyDisplayIfTrue: GameController.FindTurnTakersWhere(otherPlayers.Criteria).Any()
                ),
                new Function(
                    DecisionMaker,
                    "Place a Bug token on up to 2 Strategy cards",
                    SelectionType.AddTokens,
                    () => GameController.SelectCardsAndDoAction(
                        DecisionMaker,
                        new LinqCardCriteria(strategyWithPool, "Strategy"),
                        SelectionType.AddTokens,
                        c => GameController.AddTokensToPool(c.FindBugPool(), 1, GetCardSource()),
                        numberOfCards: 2,
                        requiredDecisions: 0,
                        cardSource: GetCardSource()
                    ),
                    onlyDisplayIfTrue: FindCardsWhere(strategyWithPool, visibleToCard: GetCardSource()).Any()
                )
            };

            var e = SelectAndPerformFunction(
                DecisionMaker,
                functions,
                noSelectableFunctionMessage: "There are no other players to draw a card and no Strategy cards to place Bug tokens on."
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator e;

            switch (index)
            {
                case 0:
                {
                    // "Select a target. Reduce the next damage dealt by that target by 2."
                    var selected = new List<SelectCardDecision>();
                    e = GameController.SelectCardAndStoreResults(
                        DecisionMaker,
                        SelectionType.ReduceDamageDealt,
                        new LinqCardCriteria(c => c.IsInPlay && c.IsTarget, "target", useCardsSuffix: false),
                        selected,
                        optional: false,
                        cardSource: GetCardSource()
                    );
                    if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                    else { GameController.ExhaustCoroutine(e); }

                    var target = GetSelectedCard(selected);
                    if (target == null) { yield break; }

                    var effect = new ReduceDamageStatusEffect(2);
                    effect.SourceCriteria.IsSpecificCard = target;
                    effect.NumberOfUses = 1;
                    effect.UntilTargetLeavesPlay(target);
                    e = AddStatusEffect(effect);
                    break;
                }
                case 1:
                {
                    // "One player may play a card."
                    e = SelectHeroToPlayCard(DecisionMaker);
                    break;
                }
                case 2:
                {
                    // "Reveal the top card of a deck, then replace it or discard it."
                    var selected = new List<SelectLocationDecision>();
                    e = GameController.SelectADeck(DecisionMaker, SelectionType.RevealTopCardOfDeck, l => true, selected, cardSource: GetCardSource());
                    if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                    else { GameController.ExhaustCoroutine(e); }

                    if (!DidSelectDeck(selected)) { yield break; }

                    e = RevealCard_DiscardItOrPutItOnDeck(DecisionMaker, TurnTakerController, GetSelectedLocation(selected), toBottom: false);
                    break;
                }
                default:
                    yield break;
            }

            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }
    }
}
