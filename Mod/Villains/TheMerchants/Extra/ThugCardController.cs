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
    public abstract class ThugCardController : CardController
    {
        public ThugCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        private DestroyCardAction _actionThatDestroyedMe;

        public abstract void AddDamageTriggers();

        public override void AddTriggers()
        {
            // "When this card would be destroyed shuffle it into the Thug deck instead"
            // This needs to trigger out-of-play because the card will be inhibited by the time it triggers.
            // We don't set the card source on the post-destroy move (because of the inhibitor)
            AddTrigger<MoveCardAction>(
                mca => mca.CardToMove == Card &&
                    mca.Destination.IsDeck &&
                    mca.Destination.IsVillain &&
                    mca.CardSource == null &&
                    mca.Origin.IsPlayArea &&
                    mca.ActionSource == _actionThatDestroyedMe,
                mca => EnsureShuffle(mca),
                TriggerType.ShuffleDeck,
                TriggerTiming.Before,
                outOfPlayTrigger: true
            );

            AddWhenDestroyedTrigger(
                SetPostDestroyLocation,
                new TriggerType[2]
                {
                    TriggerType.MoveCard,
                    TriggerType.ChangePostDestroyDestination
                },
                dca => dca.PostDestroyDestinationCanBeChanged
            );

            AddDamageTriggers();
        }

        private IEnumerator EnsureShuffle(MoveCardAction mca)
        {
            mca.ShuffleDestinationAfterMove = true;
            yield break;
        }

        private IEnumerator SetPostDestroyLocation(DestroyCardAction dca)
        {
            if (!dca.PostDestroyDestinationCanBeChanged)
            {
                yield break;
            }

            AddInhibitorException((ga) => ga is MoveCardAction || ga is MakeDecisionAction);

            // Don't set card source because actions with a card source of this card are about to get inhibited.
            // No clear point to undo an inhibitor exception.
            // This is how Crystalloid Behemoth is implemented in the base game.
            dca.SetPostDestroyDestination(GetNativeDeck(Card));
            _actionThatDestroyedMe = dca;

            RemoveInhibitorException();
        }
    }
}
