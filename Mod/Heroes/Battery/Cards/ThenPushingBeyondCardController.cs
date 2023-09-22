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
    public class ThenPushingBeyondCardController : CardController
    {
        public ThenPushingBeyondCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            // {BatteryCharacter} deals herself 3 psychic damage.
            var e = DealDamage(
                CharacterCard,
                CharacterCard,
                3,
                DamageType.Psychic
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            // You may play a card.
            e = SelectAndPlayCardFromHand(HeroTurnTakerController);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            // "You may use a power."
            e = SelectAndUsePower(this);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }
    }
}
