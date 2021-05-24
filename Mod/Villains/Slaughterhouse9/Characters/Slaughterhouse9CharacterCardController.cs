using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Slaughterhouse9
{
    public class Slaughterhouse9CharacterCardController : VillainCharacterCardController
    {
        public Slaughterhouse9CharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override void AddSideTriggers()
        {
            //  Whenever no villain targets are in play, the heroes win the game
            AddSideTrigger(AddTrigger<GameAction>(
                (g) => GameController.HasGameStarted &&
                    !(g is GameOverAction) &&
                    !(g is IncrementAchievementAction) &&
                    !(g is CancelAction) &&
                    FindCardsWhere((c) => c.IsInPlayAndHasGameText && c.IsVillainTarget).Count() == 0,
                (g) => DefeatedResponse(g),
                new TriggerType[2] { TriggerType.GameOver, TriggerType.Hidden },
                TriggerTiming.After
            ));

            // Don't let cards under this card get moved
            AddSideTrigger(AddTrigger<MoveCardAction>(
                (m) => m.CardToMove == Card || (m.Origin == Card.UnderLocation && m.Destination != TurnTaker.PlayArea),
                (m) => CancelAction(m),
                TriggerType.CancelAction,
                TriggerTiming.Before
            ));

            if (Game.IsAdvanced)
            {
                AddSideTrigger(AddEndOfTurnTrigger(
                    tt => tt == TurnTaker,
                    pca => PlayNineMember(),
                    TriggerType.PutIntoPlay
                ));
            }
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            // This card + cards under it are indestructible
            return card == Card || card.Location == Card.UnderLocation;
        }

        private IEnumerator PlayNineMember()
        {
            if (Card.UnderLocation.TopCard == null) { yield break; }

            var e = GameController.PlayCard(
                TurnTakerController,
                Card.UnderLocation.TopCard,
                isPutIntoPlay: true,
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
    }
}
