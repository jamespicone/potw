using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Lung
{
    public class BurstOfFlameCardController : CardController
    {
        public BurstOfFlameCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP(ranking: 1, numberOfTargets: 1);
            SpecialStringMaker.ShowNumberOfCardsAtLocation(base.TurnTaker.Trash);
        }

        public override System.Collections.IEnumerator Play()
        {
            // Lung deals the hero target with the highest HP X fire damage, where X = 1 + the number of cards in the villain trash / 2
            int damage = 1 + TurnTaker.Trash.NumberOfCards / 2;
            var e = DealDamageToHighestHP(TurnTaker.CharacterCard, 1, c => c.IsHero && c.IsTarget && c.IsInPlay, c => damage, DamageType.Fire);
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
