using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.TheMerchants
{
    public class TheMerchantsTurnTakerController : TurnTakerController
    {
        public TheMerchantsTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {
        }

        public override void AddTriggers()
        {
            if (CharacterCard.IsFlipped || ! CharacterCard.IsInPlay)
            {
                AddGameEndTrigger();
            }
            else
            {
                GameController.AddTrigger(new Trigger<FlipCardAction>(
                    GameController,
                    fca => fca.CardToFlip == CharacterCardController,
                    fca => SkidmarkDefeated(),
                    new TriggerType[] { TriggerType.GameOver, TriggerType.Hidden },
                    TriggerTiming.After,
                    CharacterCardController.GetCardSource()
                ));

                GameController.AddTrigger(new Trigger<MoveCardAction>(
                    GameController,
                    mca => mca.CardToMove == CharacterCard && !mca.Destination.IsInPlay,
                    mca => SkidmarkDefeated(),
                    new TriggerType[] { TriggerType.GameOver, TriggerType.Hidden },
                    TriggerTiming.After,
                    CharacterCardController.GetCardSource()
                ));
            }
        }

        private bool HeroesWon()
        {
            return (CharacterCard.IsFlipped || ! CharacterCard.IsInPlayAndHasGameText) &&
                ! GameController.GetAllCards().Any(c => c.IsInPlayAndHasGameText && c.Is(CharacterCardController).Villain().Target());
        }

        private IEnumerator MerchantsGameOver()
        {
            var e = GameController.GameOver(
                EndingResult.AlternateVictory, 
                $"The heroes have defeated {CharacterCard.Title} and rounded up all of the Merchants!\nThe day is saved!",
                showEndingTextAsMessage: true
            );
            if (UseUnityCoroutines) yield return GameController.StartCoroutine(e);
            else GameController.ExhaustCoroutine(e);
        }

        private bool _destroyedMessageShown = false;

        private IEnumerator SkidmarkDefeated()
        {
            if (_destroyedMessageShown) yield break;

            if (HeroesWon())
            {
                var e = MerchantsGameOver();
                if (UseUnityCoroutines) yield return GameController.StartCoroutine(e);
                else GameController.ExhaustCoroutine(e);
            }

            if (! GameController.IsGameOver)
            {
                var e = GameController.SendMessageAction(
                    $"{CharacterCard.Title} has been defeated, but his followers continue to rampage as long as there is a villain target in play!",
                    Priority.Critical,
                    CharacterCardController.GetCardSource(),
                    GameController.FindCardsWhere(new LinqCardCriteria((Card c) => c.Is(CharacterCardController).Villain().Target() && c.IsInPlayAndHasGameText), visibleToCard: CharacterCardController.GetCardSource()),
                    showCardSource: true
                );
                if (UseUnityCoroutines) yield return GameController.StartCoroutine(e);
                else GameController.ExhaustCoroutine(e);

                AddGameEndTrigger();

                _destroyedMessageShown = true;
            }
        }

        private void AddGameEndTrigger()
        {
            GameController.AddTrigger(new Trigger<GameAction>(
                GameController,
                ga => HeroesWon() && ! (ga is GameOverAction) && ! (ga is MessageAction),
                ga => MerchantsGameOver(),
                new TriggerType[] { TriggerType.GameOver, TriggerType.Hidden },
                TriggerTiming.After,
                cardSource: CharacterCardController.GetCardSource()
            ));
        }
    }
}
