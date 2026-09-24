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
    public class SkitterKhepriCharacterCardController : HeroCharacterCardController
    {
        public SkitterKhepriCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddSideTriggers()
        {
            if (! Card.IsFlipped)
            {
                // "At the start of your turn, place a Bug token on {SkitterCharacter}."
                AddSideTrigger(AddStartOfTurnTrigger(
                    tt => tt == TurnTaker && Card.IsInPlayAndNotUnderCard,
                    pca => this.AddBugTokenToSkitter(1),
                    TriggerType.AddTokensToPool
                ));
            }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            var skitter = CharacterCard ?? Card;

            // "Select a hero other than {SkitterCharacter}. That hero uses a power."
            var selectedHero = new List<SelectTurnTakerDecision>();
            var e = GameController.SelectHeroToUsePower(
                DecisionMaker,
                optionalSelectHero: false,
                optionalUsePower: false,
                storedResultsDecision: selectedHero,
                additionalCriteria: new LinqTurnTakerCriteria(tt => tt != TurnTaker && tt != skitter.Owner, $"hero other than {skitter.Title}"),
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            var hero = GetSelectedTurnTaker(selectedHero);

            // "Then either remove a Bug token from one of your cards or remove one of your non-character cards in play from the game."
            Func<Card, bool> isYours = c => skitter.Owner.IsPlayer ? c.Owner == skitter.Owner : c == skitter;
            Func<Card, bool> hasTokens = c => c.IsInPlay && isYours(c) && c.BugTokenCount() > 0;
            Func<Card, bool> removable = c => c.IsInPlay && isYours(c) && !c.IsCharacter;

            var removedCard = new List<Card>();
            var functions = new List<Function>
            {
                new Function(
                    DecisionMaker,
                    "Remove a Bug token from one of your cards",
                    SelectionType.RemoveTokens,
                    () => RemoveBugToken(hasTokens),
                    onlyDisplayIfTrue: FindCardsWhere(hasTokens, visibleToCard: GetCardSource()).Any()
                ),
                new Function(
                    DecisionMaker,
                    "Remove one of your non-character cards in play from the game",
                    SelectionType.RemoveCardFromGame,
                    () => RemoveCardFromGame(removable, removedCard),
                    onlyDisplayIfTrue: FindCardsWhere(removable, visibleToCard: GetCardSource()).Any()
                )
            };

            e = SelectAndPerformFunction(
                DecisionMaker,
                functions,
                noSelectableFunctionMessage: $"{skitter.Title} has no Bug tokens to remove and no non-character cards in play."
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            // "If you remove a card this way, {SkitterCharacter} deals that hero 2 irreducible psychic damage."
            if (removedCard.Any() && hero != null)
            {
                e = GameController.SelectTargetsAndDealDamage(
                    DecisionMaker,
                    new DamageSource(GameController, skitter),
                    2,
                    DamageType.Psychic,
                    numberOfTargets: 1,
                    optional: false,
                    requiredTargets: 1,
                    isIrreducible: true,
                    additionalCriteria: c => c.Owner == hero && c.IsHeroCharacterCard && !c.IsIncapacitatedOrOutOfGame,
                    cardSource: GetCardSource()
                );
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }
        }

        private IEnumerator RemoveBugToken(Func<Card, bool> hasTokens)
        {
            var selected = new List<SelectCardDecision>();
            var e = GameController.SelectCardAndStoreResults(
                DecisionMaker,
                SelectionType.RemoveTokens,
                new LinqCardCriteria(hasTokens, "card with Bug tokens", useCardsSuffix: false),
                selected,
                optional: false,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            var card = GetSelectedCard(selected);
            if (card == null) { yield break; }

            e = GameController.RemoveTokensFromPool(card.FindBugPool(), 1, cardSource: GetCardSource());
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }

        private IEnumerator RemoveCardFromGame(Func<Card, bool> removable, List<Card> removedCard)
        {
            var selected = new List<SelectCardDecision>();
            var e = GameController.SelectCardAndStoreResults(
                DecisionMaker,
                SelectionType.RemoveCardFromGame,
                new LinqCardCriteria(removable, "non-character"),
                selected,
                optional: false,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            var card = GetSelectedCard(selected);
            if (card == null) { yield break; }

            var moves = new List<MoveCardAction>();
            e = GameController.MoveCard(
                TurnTakerController,
                card,
                card.Owner.OutOfGame,
                responsibleTurnTaker: TurnTaker,
                storedResults: moves,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            if (moves.Any(m => m.WasCardMoved))
            {
                removedCard.Add(card);
            }
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator e;

            switch (index)
            {
                case 0:
                {
                    // "One hero may use a power."
                    e = GameController.SelectHeroToUsePower(DecisionMaker, cardSource: GetCardSource());
                    break;
                }
                case 1:
                {
                    // "One player may destroy one of their non-character cards. If they do, they draw a card and prevent the next damage that would be dealt to a hero target."
                    var destroyed = new List<DestroyCardAction>();
                    e = GameController.SelectHeroToDestroyTheirCard(
                        DecisionMaker,
                        new LinqCardCriteria(c => !c.IsCharacter, "non-character"),
                        optionalSelectHero: false,
                        optionalDestroyCard: true,
                        storedResults: destroyed,
                        responsibleCard: Card,
                        cardSource: GetCardSource()
                    );
                    if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                    else { GameController.ExhaustCoroutine(e); }

                    if (!DidDestroyCard(destroyed)) { yield break; }

                    e = DrawCard(destroyed.First().CardToDestroy.Card.Owner.ToHero());
                    if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                    else { GameController.ExhaustCoroutine(e); }

                    var effect = new CannotDealDamageStatusEffect();
                    effect.TargetCriteria.IsHero = true;
                    effect.TargetCriteria.IsTarget = true;
                    effect.NumberOfUses = 1;
                    effect.IsPreventEffect = true;
                    e = AddStatusEffect(effect);
                    break;
                }
                case 2:
                {
                    // "Select a non-hero, non-character target. That target deals 1 other target 1 melee damage, then deals itself 1 irreducible psychic damage."
                    var selected = new List<SelectCardDecision>();
                    e = GameController.SelectCardAndStoreResults(
                        DecisionMaker,
                        SelectionType.CardToDealDamage,
                        new LinqCardCriteria(c => c.IsInPlay && c.IsTarget && !c.Is(this).Hero() && !c.IsCharacter, "non-hero, non-character target", useCardsSuffix: false),
                        selected,
                        optional: false,
                        cardSource: GetCardSource()
                    );
                    if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                    else { GameController.ExhaustCoroutine(e); }

                    var puppet = GetSelectedCard(selected);
                    if (puppet == null) { yield break; }

                    e = GameController.SelectTargetsAndDealDamage(
                        DecisionMaker,
                        new DamageSource(GameController, puppet),
                        1,
                        DamageType.Melee,
                        numberOfTargets: 1,
                        optional: false,
                        requiredTargets: 1,
                        additionalCriteria: c => c != puppet,
                        cardSource: GetCardSource()
                    );
                    if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                    else { GameController.ExhaustCoroutine(e); }

                    if (!puppet.IsInPlayAndHasGameText || !puppet.IsTarget) { yield break; }

                    e = GameController.DealDamageToTarget(new DamageSource(GameController, puppet), puppet, 1, DamageType.Psychic, isIrreducible: true, cardSource: GetCardSource());
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
