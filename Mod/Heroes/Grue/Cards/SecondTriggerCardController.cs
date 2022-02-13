using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Grue
{
    public class SecondTriggerCardController : CardController
    {
        public SecondTriggerCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            //"For the rest of the game increase damage dealt to {GrueCharacter} by 1",
            var effect = new IncreaseDamageStatusEffect(1);
            effect.TargetCriteria.IsSpecificCard = CharacterCard;

            var e = AddStatusEffect(effect);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            //"For the rest of the game you may use {Trigger} powers",
            CharacterCardController.SetGrueCanUseTriggerPowers();
            e = GameController.SendMessageAction(
                $"For the rest of the game {HeroTurnTaker.NameRespectingVariant} can use {{Trigger}} powers",
                Priority.High,
                GetCardSource(),
                new Card[] { CharacterCard },
                showCardSource: true
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            //"Remove this card from the game"
            e = GameController.MoveCard(
                TurnTakerController,
                Card,
                TurnTaker.OutOfGame,
                showMessage: true,
                responsibleTurnTaker: TurnTaker,
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

        public override bool DoNotMoveOneShotToTrash => true;
    }
}
