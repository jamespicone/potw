using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Leviathan
{
    public class CunningCardController : CardController
    {
        public CunningCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }
        
        public override IEnumerator Play()
        {
            // Play the top 2 cards of the villain deck

            var e1 = PlayTheTopCardOfTheVillainDeckResponse(null);
            var e2 = PlayTheTopCardOfTheVillainDeckResponse(null);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e1);
                yield return GameController.StartCoroutine(e2);
            }
            else
            {
                GameController.ExhaustCoroutine(e1);
                GameController.ExhaustCoroutine(e2);
            }
        }

    }
}
