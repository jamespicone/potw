using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Leviathan
{
    public class AnyoneCanDieController : CardController
    {
        public AnyoneCanDieController(Card card, TurnTakerController controller) : base(card, controller)
        { }
        
        public override System.Collections.IEnumerator Play()
        {
            // Leviathan deals the hero target with the highest HP 5 irreducible melee damage
            var e = DealDamageToHighestHP(TurnTaker.CharacterCard, 1, c => c.IsHero && c.IsTarget && c.IsInPlay, c => 5, DamageType.Melee, true);
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
