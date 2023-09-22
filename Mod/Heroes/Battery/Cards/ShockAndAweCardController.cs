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
    public class ShockAndAweCardController : CardController
    {
        public ShockAndAweCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.ShowChargedStatus(SpecialStringMaker, CharacterCard);
        }

        public override IEnumerator Play()
        {
            // {BatteryCharacter} deals 1 target 2 lightning damage.
            var firstStrike = new List<DealDamageAction>();
            var e = GameController.SelectTargetsAndDealDamage(
                HeroTurnTakerController,
                new DamageSource(GameController, CharacterCard),
                2,
                DamageType.Lightning,
                numberOfTargets: 1,
                optional: false,
                requiredTargets: 1,
                storedResultsDamage: firstStrike,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            // If {BatteryCharacter} is {Charged}, she deals up to 3 other targets 3 lightning damage each.
            if (this.IsCharged(CharacterCard))
            {
                var previouslySelected = firstStrike.Select(dda => dda.OriginalTarget);
                e = GameController.SelectTargetsAndDealDamage(
                    HeroTurnTakerController,
                    new DamageSource(GameController, CharacterCard),
                    3,
                    DamageType.Lightning,
                    numberOfTargets: 3,
                    optional: false, 
                    requiredTargets: 0,
                    additionalCriteria: c => ! previouslySelected.Contains(c),
                    cardSource: GetCardSource()
                );
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }
        }
    }
}
