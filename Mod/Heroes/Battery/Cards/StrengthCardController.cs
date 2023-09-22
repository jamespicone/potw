using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Battery
{
    public class StrengthCardController : CardController
    {
        public StrengthCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.ShowChargedStatus(SpecialStringMaker, CharacterCard);
        }

        public override IEnumerator Play()
        {
            // {BatteryCharacter} deals 1 target 3 melee damage.
            List<DealDamageAction> storedResults = new List<DealDamageAction>();
            var e = GameController.SelectTargetsAndDealDamage(
                HeroTurnTakerController,
                new DamageSource(GameController, CharacterCard),
                3,
                DamageType.Melee,
                numberOfTargets: 1,
                optional: false,
                requiredTargets: 1,
                storedResultsDamage: storedResults,
                cardSource: GetCardSource()
            );

            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            // If {BatteryCharacter} is {Charged}, she deals the same target 2 more melee damage.
            var target = storedResults.FirstOrDefault()?.Target;
            if (target != null && this.IsCharged(CharacterCard))
            {
                e = DealDamage(
                    CharacterCard,
                    target,
                    2,
                    DamageType.Melee
                );

                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }
        }
    }
}
