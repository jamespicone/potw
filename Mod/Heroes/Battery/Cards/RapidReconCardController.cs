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
    public class RapidReconCardController : BatteryUtilityCardController
    {
        public RapidReconCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.ShowChargedStatus(SpecialStringMaker, CharacterCard);
        }

        public override IEnumerator Play()
        {
            IEnumerator e;

            // If {BatteryCharacter} is {Discharged}, each player may draw a card.
            if (! this.IsCharged(CharacterCard))
            {
                e = EachPlayerDrawsACard(optional: true);
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }

            // "Draw a card."
            e = DrawCard(HeroTurnTaker);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            // "You may play a card."
            e = SelectAndPlayCardFromHand(HeroTurnTakerController);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }
    }
}
