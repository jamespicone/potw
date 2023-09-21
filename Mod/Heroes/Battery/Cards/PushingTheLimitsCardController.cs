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
    public class PushingTheLimitsCardController : BatteryUtilityCardController
    {
        public PushingTheLimitsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.ShowChargedStatus(SpecialStringMaker, CharacterCard);
        }

        public override IEnumerator Play()
        {
            // If {BatteryCharacter} is {Charged}, you may play up to 2 cards.
            if (this.IsCharged(CharacterCard))
            {
                var e = SelectAndPlayCardsFromHand(HeroTurnTakerController, numberOfCards: 2);
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }

            // If {BatteryCharacter} is {Discharged}, draw 3 cards.
            if (!this.IsCharged(CharacterCard))
            {
                var e = DrawCards(HeroTurnTakerController, 3);
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }
        }
    }
}
