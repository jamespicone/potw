using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Leviathan
{
    public class LoseTheWarCardController : CardController
    {
        public LoseTheWarCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            // This card is indestructible
            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            return card == Card;
        }

        public override void AddTriggers()
        {
            // If the environment deck is ever empty, the heroes lose the game.
            AddTrigger<GameAction>(
                ga => !(ga is GameOverAction) && FindEnvironment().TurnTaker.Deck.IsEmpty,
                ga => GameOver(ga),
                TriggerType.GameOver,
                TriggerTiming.After
            );

            // At the end of the villain turn, remove the top card of the environment deck from the game
            AddEndOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => ExileEnvironment(pca),
                TriggerType.RemoveFromGame
            );

            // Players may skip any of their phases (play, power, draw).
            AddPhaseChangeTrigger(
                tt => tt.IsHero,
                p => p == Phase.PlayCard || p == Phase.UsePower || p == Phase.DrawCard,
                pca => true,
                pca => ShoreUpEnvironment(pca),
                new TriggerType[] { TriggerType.SkipPhase },
                TriggerTiming.Before
            );
        }

        public override IEnumerator Play()
        {
            // Leviathan deals the hero target with the highest HP 5 irreducible melee damage
            var e = DealDamageToHighestHP(TurnTaker.CharacterCard, 1, c => c.IsHero && c.IsTarget && c.IsInPlay, c => 5, DamageType.Melee, true);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        private IEnumerator GameOver(GameAction ga)
        {
            var e = GameController.GameOver(
                EndingResult.AlternateDefeat,
                "Leviathan has destroyed your surroundings!",
                actionSource: ga,
                cardSource: GetCardSource()
            );

            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        private IEnumerator ExileEnvironment(PhaseChangeAction pca)
        {
            var cardToMove = FindEnvironment().TurnTaker.Deck.TopCard;
            if (cardToMove == null) { yield break; }

            var e = GameController.MoveCard(
                TurnTakerController,
                cardToMove,
                TurnTaker.OutOfGame,
                showMessage: true,
                responsibleTurnTaker: TurnTaker,
                actionSource: pca,
                cardSource: GetCardSource()
            );

            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        private IEnumerator ShoreUpEnvironment(PhaseChangeAction pca)
        {
            var hero = pca.ToPhase.TurnTaker.ToHero();
            if (hero == null) { yield break; }

            var heroController = FindHeroTurnTakerController(hero);
            if (heroController == null) { yield break; }

            var stored = new List<YesNoCardDecision>();
            var e = GameController.MakeYesNoCardDecision(
                heroController,
                SelectionType.Custom,
                Card,
                pca,
                stored,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            var decision = stored.FirstOrDefault();
            if (decision == null) { yield break; }

            if (!DidPlayerAnswerYes(decision)) { yield break; }

            // Any time a player skips a phase, they may take a card from the environment trash and shuffle it back into the environment deck
            e = GameController.SelectAndMoveCard(
                DecisionMaker,
                c => c.IsEnvironment && c.Location.IsTrash,
                FindEnvironment().TurnTaker.Deck,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            e = GameController.ShuffleLocation(FindEnvironment().TurnTaker.Deck, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            var nextPhase = GameController.FindNextTurnPhase(pca.ToPhase);
            e = GameController.SkipToTurnPhase(nextPhase, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {
            return new CustomDecisionText(
                "Skip the next phase?",
                decision.HeroTurnTakerController.CharacterCard.Title + " is deciding whether to skip a phase",
                "Vote for skipping the next phase",
                "Whether to skip the next phase"
            );
        }
    }
}
