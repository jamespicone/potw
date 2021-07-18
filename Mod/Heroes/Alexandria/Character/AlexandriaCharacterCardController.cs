using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Alexandria
{
    public class AlexandriaCharacterCardController : HeroCharacterCardController
    {
        public AlexandriaCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            //"Select a target. Until the start of your next turn reduce damage dealt to that target by 1",
            //"One player may search their deck for a card and put it into their hand",
            //"{AlexandriaCharacter} deals 2 melee damage to a target"
            yield break;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // You may play a card.
            var e = SelectAndPlayCardFromHand(HeroTurnTakerController);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            // Return one of your cards in play to your hand
            e = GameController.SelectAndMoveCard(
                HeroTurnTakerController,
                c => !c.IsCharacter && c.IsInPlay && c.Location == TurnTaker.PlayArea,
                HeroTurnTaker.Hand,
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
