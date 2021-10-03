using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.Leviathan
{
    public class WhippingTailCardController : CardController
    {
        public WhippingTailCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }
        
        public override IEnumerator Play()
        {
            // {LeviathanCharacter} deals 2 melee damage to the 2 hero targets with the lowest HP
            var e = DealDamageToLowestHP(
                CharacterCard,
                1,
                c => this.HasAlignment(c, CardAlignment.Hero, CardTarget.Target) && c.IsInPlay,
                c => 2,
                DamageType.Melee,
                numberOfTargets: 2
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
