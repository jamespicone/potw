using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Leviathan
{
    public class AnyoneCanDieCardController : CardController
    {
        public AnyoneCanDieCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }
        
        public override IEnumerator Play()
        {
            // Leviathan deals the hero target with the highest HP 5 irreducible melee damage
            var e = DealDamageToHighestHP(
                CharacterCard,
                ranking: 1,
                c => c.Is(this).Hero().Target() && c.IsInPlay,
                c => 5,
                DamageType.Melee,
                isIrreducible: true
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
