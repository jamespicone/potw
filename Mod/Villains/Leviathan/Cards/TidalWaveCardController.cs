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
    public class TidalWaveCardController : CardController
    {
        public TidalWaveCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }
        
        public override IEnumerator Play()
        {
            // Destroy all environment targets. {LeviathanCharacter} deals X cold damage to all non-villain targets, where X = 1 + the number of cards destroyed in this way
            var stored = new List<DestroyCardAction>();
            var e = GameController.DestroyCards(
                DecisionMaker,
                new LinqCardCriteria(c => c.Alignment().Environment().Target(), "environment targets"),
                storedResults: stored,
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

            var x = 1 + stored.Count(dca => dca.IsSuccessful);

            e = GameController.DealDamage(
                DecisionMaker,
                CharacterCard,
                c => c.IsTarget && c.IsInPlay && !c.IsVillainTarget,
                amount: x,
                DamageType.Cold,
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
