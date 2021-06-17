using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Leviathan
{
    public class GuileCardController : CardController
    {
        public GuileCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }
        
        public override IEnumerator Play()
        {
            // Flip {LeviathanCharacter}
            var e = GameController.FlipCard(
                CharacterCardController,
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
