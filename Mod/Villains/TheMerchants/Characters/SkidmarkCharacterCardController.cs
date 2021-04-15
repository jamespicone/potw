using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.TheMerchants
{
    class SkidmarkCharacterCardController : VillainCharacterCardController
    {
        public SkidmarkCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            ThugDeck = base.Card.UnderLocation;
            SpecialStringMaker.ShowNumberOfCardsAtLocation(ThugDeck);
        }

        public override bool CanBeDestroyed => false;

        public Location ThugDeck
        {
            get;
            private set;
        }

        public const string HeroesCannotWinMessage = "HeroesCannotWinMessage";
        public bool IsGameEnding = false;

        public override void AddSideTriggers()
        {
            base.AddSideTriggers();
            if (!base.Card.IsFlipped)
            {
                // Superpowered Drug Lord
                // "At the start of the villain turn, if the Thug deck is empty, the Merchants have grown out of control. [b]GAME OVER.[/b]"
                base.AddSideTrigger(AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, GameOverResponse, TriggerType.GameOver, additionalCriteria: (PhaseChangeAction pca) => !ThugDeck.HasCards));
                // "At the end of the villain turn, play the top {H - 1} cards of the Thug deck."
                base.AddSideTrigger(AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => base.GameController.PlayTopCardOfLocation(base.TurnTakerController, ThugDeck, numberOfCards: H - 1, responsibleTurnTaker: base.TurnTaker, cardSource: GetCardSource()), TriggerType.PlayCard));
                if (IsGameAdvanced)
                {
                    // "Reduce damage dealt to villain targets by 1."
                    base.AddSideTrigger(AddReduceDamageTrigger((Card c) => c.IsVillain, 1));
                }
            }
            else
            {
                // Defeated Drug Lord
                // "[b]The heroes cannot win the game as long as there is a villain target in play.[/b]"
                base.AddSideTrigger(AddTrigger((GameOverAction goa) => goa.ResultIsVictory && base.GameController.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsVillain && c.IsTarget && c.IsInPlayAndHasGameText), visibleToCard: GetCardSource()).Any(), CancelVictoryResponse, TriggerType.CancelAction, TriggerTiming.Before));
                // "At the start of the villain turn, if the Thug deck is empty, the Merchants have grown out of control. [b]GAME OVER.[/b]"
                base.AddSideTrigger(AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, GameOverResponse, TriggerType.GameOver, additionalCriteria: (PhaseChangeAction pca) => !ThugDeck.HasCards));
                // "At the end of the villain turn, play the top card of the Thug deck."
                base.AddSideTrigger(AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => base.GameController.PlayTopCardOfLocation(base.TurnTakerController, ThugDeck, responsibleTurnTaker: base.TurnTaker, cardSource: GetCardSource()), TriggerType.PlayCard));
                // If there are ever no villain targets in play, the heroes win
                //base.AddSideTrigger(AddTrigger((GameAction ga) => DidHeroesWin(ga), VictoryResponse, new TriggerType[] { TriggerType.GameOver, TriggerType.Hidden }, TriggerTiming.After));
            }
            // If there are ever no villain targets in play, the heroes win
            AddTrigger((GameAction ga) => !IsGameEnding && DidHeroesWin(ga), VictoryResponse, new TriggerType[] { TriggerType.GameOver, TriggerType.Hidden }, TriggerTiming.After);
            AddDefeatedIfMovedOutOfGameTriggers();
            ThugDeck.OverrideIsInPlay = false;
        }

        public override IEnumerator DestroyAttempted(DestroyCardAction destroyCard)
        {
            // "When {SkidmarkCharacter} would be destroyed, flip {SkidmarkCharacter}'s villain character cards instead."
            //Log.Debug("SkidmarkCharacterCardController.DestroyAttempted activated...");
            if (!base.Card.IsFlipped)
            {
                //Log.Debug("Flipping " + base.Card.Title + " instead of destroying him...");
                IEnumerator removeCoroutine = base.GameController.RemoveTarget(base.Card, cardSource: GetCardSource());
                IEnumerator flipCoroutine = base.GameController.FlipCard(this, treatAsPlayed: false, treatAsPutIntoPlay: false, destroyCard.ActionSource, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(removeCoroutine);
                    yield return base.GameController.StartCoroutine(flipCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(removeCoroutine);
                    base.GameController.ExhaustCoroutine(flipCoroutine);
                }
            }
            yield break;
        }

        public IEnumerator GameOverResponse(GameAction ga)
        {
            // "... the Merchants have grown out of control. [b]GAME OVER.[/b]"
            IsGameEnding = true;
            string ending = "The Thug deck is empty! The Merchants have become too numerous for the heroes to contain!";
            IEnumerator endCoroutine = base.GameController.GameOver(EndingResult.AlternateDefeat, ending, showEndingTextAsMessage: true, actionSource: ga, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(endCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(endCoroutine);
            }
            yield break;
        }

        public IEnumerator CancelVictoryResponse(GameOverAction goa)
        {
            // "[b]The heroes cannot win the game as long as there is a villain target in play.[/b]"
            if (!HasBeenSetToTrueThisGame(nameof(HeroesCannotWinMessage)))
            {
                IEnumerator messageCoroutine = base.GameController.SendMessageAction(base.Card.Title + " has been defeated, but his followers continue to rampage as long as there is a villain target in play!", Priority.Critical, GetCardSource(), base.GameController.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsTarget && c.IsVillain && c.IsInPlayAndHasGameText), visibleToCard: GetCardSource()), showCardSource: true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(messageCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(messageCoroutine);
                }
                SetCardPropertyToTrueIfRealAction(nameof(HeroesCannotWinMessage));
            }
            IEnumerator cancelCoroutine = CancelAction(goa);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(cancelCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(cancelCoroutine);
            }
            yield break;
        }

        private bool DidHeroesWin(GameAction ga)
        {
            //Log.Debug("Checking if the heroes have won yet...");
            if (!base.GameController.AllHeroControllers.All((HeroTurnTakerController httc) => httc.IsIncapacitatedOrOutOfGame))
            {
                //Log.Debug("There's at least one active hero...");
                IEnumerable<Card> activeVillainTargets = base.GameController.GetAllCards().Where((Card c) => c.IsInPlayAndHasGameText && c.IsVillainTarget);
                Log.Debug("activeVillainTargets.Count(): " + activeVillainTargets.Count().ToString());
                if (activeVillainTargets.Count() > 0)
                {
                    foreach (Card c in activeVillainTargets)
                    {
                        Log.Debug("active villain target: " + c.Title);
                    }
                }
                return !base.GameController.GetAllCards().Any((Card c) => c.IsInPlayAndHasGameText && c.IsVillainTarget);
            }
            else
            {
                return false;
            }
        }

        public IEnumerator VictoryResponse(GameAction ga)
        {
            // If Skidmark is flipped and there are no villain targets in play, the heroes win
            IsGameEnding = true;
            Log.Debug("The heroes have defeated " + base.Card.Title + " and rounded up all of the Merchants!\nThe day is saved! Let's see if the code agrees...");
            IEnumerator endCoroutine = base.GameController.GameOver(EndingResult.AlternateVictory, "The heroes have defeated " + base.Card.Title + " and rounded up all of the Merchants!\nThe day is saved!", showEndingTextAsMessage: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(endCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(endCoroutine);
            }
            yield break;
        }
    }
}
