using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Jp.SOTMUtilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Battery
{
    public class NoTimeToRestCardController : CardController
    {
        public NoTimeToRestCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // Whenever {BatteryCharacter} destroys a target
            AddTrigger<DestroyCardAction>(
                dca => dca.WasCardDestroyed && dca.CardToDestroy.Card.IsTarget && TurnTaker.IsResponsible(dca),
                DrawPlayResponse,
                new TriggerType[] { TriggerType.DrawCard, TriggerType.PlayCard },
                TriggerTiming.After
            );
        }

        public IEnumerator DrawPlayResponse(DestroyCardAction dca)
        {
            // "... you may draw a card and you may play a card."
            var e = DrawCard(optional: true);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            e = SelectAndPlayCardFromHand(HeroTurnTakerController);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }
    }
}
