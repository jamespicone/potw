using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Echidna
{
    public class BullrushCardController : CardController
    {
        public BullrushCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            // Echidna deals {H + 2} melee damage to the hero target with the lowest HP.
            var e = DealDamageToLowestHP(
                CharacterCard,
                1,
                c => c.Is().Hero().Target(),
                c => H + 2,
                DamageType.Melee
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            // Search the villain deck for an 'Engulfed' card and put it into play.
            // Shuffle the villain deck.
            e = PlayCardFromLocation(TurnTaker.Deck, "Engulfed");
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }
    }
}
