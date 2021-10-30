using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.TheSimurgh
{
    public class ATragicEndCardController : CardController
    {
        public ATragicEndCardController(Card card, TurnTakerController controller) : base(card, controller)
        {}

        public override IEnumerator Play()
        {
            // "When this card is flipped face up, destroy all non-character card targets.
            var results = new List<DestroyCardAction>();
            var e = GameController.DestroyCards(
                DecisionMaker,
                new LinqCardCriteria(c => c.Is().Noncharacter().Target(), "non-character targets", useCardsSuffix: false),
                storedResults: results,
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

            // {TheSimurghCharacter} deals each hero target X projectile damage, where X is the number of targets destroyed this way.",
            var destroyedCount = results.Count(dca => dca.WasCardDestroyed);

            e = DealDamage(CharacterCard, c => c.Is().Hero().Target(), destroyedCount, DamageType.Projectile);
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
