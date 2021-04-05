using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Battery
{
    public class NoTimeToRestCardController : BatteryUtilityCardController
    {
        public NoTimeToRestCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // "Whenever {BatteryCharacter} destroys a target, you may draw a card and you may play a card."
            base.AddTrigger<DestroyCardAction>((DestroyCardAction dca) => dca.WasCardDestroyed && dca.CardToDestroy.Card.IsTarget && dca.ResponsibleCard.Owner.CharacterCard == base.CharacterCard, DrawPlayResponse, new TriggerType[] { TriggerType.DrawCard, TriggerType.PlayCard }, TriggerTiming.After);
            base.AddTriggers();
        }

        public IEnumerator DrawPlayResponse(DestroyCardAction dca)
        {
            // "... you may draw a card and you may play a card."
            IEnumerator drawCoroutine = base.GameController.DrawCard(base.HeroTurnTaker, optional: true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(drawCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(drawCoroutine);
            }
            IEnumerator playCoroutine = base.GameController.SelectAndPlayCardFromHand(base.HeroTurnTakerController, true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(playCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(playCoroutine);
            }
            yield break;
        }
    }
}
