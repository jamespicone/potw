using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.MissMilitia
{
    public class GrenadeCardController : MissMilitiaUtilityCardController
    {
        public GrenadeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            // "{MissMilitiaCharacter} deals a non-hero target 2 fire damage and 2 melee damage."
            var storedResultsDamage = new List<DealDamageAction>();
            
            var attacks = new List<DealDamageAction>();
            attacks.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, CharacterCard), target: null, amount: 2, DamageType.Fire));
            attacks.Add(new DealDamageAction(GetCardSource(), new DamageSource(GameController, CharacterCard), target: null, amount: 2, DamageType.Melee));

            var e = SelectTargetsAndDealMultipleInstancesOfDamage(
                attacks,
                (c) => ! c.IsHeroTarget(),
                minNumberOfTargets: 1,
                maxNumberOfTargets: 1,
                storedResultsAction: storedResultsDamage
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            // "If she dealt damage this way, you may destroy an Ongoing card."
            if (DidDealDamage(storedResultsDamage))
            {
                e = GameController.SelectAndDestroyCard(
                    HeroTurnTakerController,
                    new LinqCardCriteria((c) => c.DoKeywordsContain("ongoing"), "ongoing"),
                    optional: true,
                    responsibleCard: Card,
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
}
