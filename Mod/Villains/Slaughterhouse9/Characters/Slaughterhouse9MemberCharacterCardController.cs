using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Slaughterhouse9
{
    public class Slaughterhouse9MemberCharacterCardController : VillainCharacterCardController
    {
        public override bool CanBeDestroyed => false;

        public Slaughterhouse9MemberCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
        }

        protected ITrigger AddAttackTrigger(Func<IEnumerator> response, IEnumerable<TriggerType> triggerTypes, string name)
        {
            return AddNineTrigger(response, triggerTypes, name, "attack");
        }

        protected ITrigger AddDefenceTrigger(Func<IEnumerator> response, IEnumerable<TriggerType> triggerTypes, string name)
        {
            return AddNineTrigger(response, triggerTypes, name, "defence");
        }

        protected ITrigger AddSpecialTrigger(Func<IEnumerator> response, IEnumerable<TriggerType> triggerTypes, string name)
        {
            return AddNineTrigger(response, triggerTypes, name, "special");
        }

        private ITrigger AddNineTrigger(Func<IEnumerator> response, IEnumerable<TriggerType> triggerTypes, string name, string keyword)
        {
            return AddTrigger<MoveCardAction>(
                mca =>
                    mca.Destination == TurnTaker.Trash &&
                    mca.CardToMove.DoKeywordsContain(keyword) &&
                    ! HasBeenSetToTrueThisTurn(name),
                mca => DoCardResponse(mca, response, name),
                triggerTypes,
                TriggerTiming.After
            );
        }

        private IEnumerator DoCardResponse(MoveCardAction mca, Func<IEnumerator> response, string name)
        {
            SetCardPropertyToTrueIfRealAction(name, gameAction: mca);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(response());
            }
            else
            {
                GameController.ExhaustCoroutine(response());
            }
        }

        public override IEnumerator BeforeFlipCardImmediateResponse(FlipCardAction flip)
        {
            var cardSource = flip.CardSource ?? (flip.ActionSource?.CardSource ?? GetCardSource());

            var e = GameController.RemoveTarget(Card, leavesPlayIfInPlay: true, cardSource);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        public override IEnumerator DestroyAttempted(DestroyCardAction destroyCard)
        {
            var action = new FlipCardAction(
                GameController,
                this,
                treatAsPlayedIfFaceUp: false,
                treatAsPutIntoPlayIfFaceUp: false,
                destroyCard.ActionSource
            );

            var e = DoAction(action);
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
