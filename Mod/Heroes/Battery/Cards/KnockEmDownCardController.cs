using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Battery
{
    public class KnockEmDownCardController : BatteryUtilityCardController
    {
        public KnockEmDownCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.ShowChargedStatus(SpecialStringMaker, CharacterCard);
        }

        public override IEnumerator Play()
        {
            // {BatteryCharacter} deals 1 non-hero target 2 melee damage.
            var e = GameController.SelectTargetsAndDealDamage(
                HeroTurnTakerController,
                new DamageSource(GameController, CharacterCard),
                2,
                DamageType.Melee,
                numberOfTargets: 1,
                optional: false,
                requiredTargets: 1, 
                additionalCriteria: c => c.Is().NonHero().Target().AccordingTo(this),
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            // If {BatteryCharacter} is {Charged}, she deals each non-hero target 2 melee damage.
            if (this.IsCharged(CharacterCard))
            {
                e = DealDamage(
                    CharacterCard,
                    c => c.Is(this).NonHero().Target(),
                    2,
                    DamageType.Melee
                );
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }
        }
    }
}
