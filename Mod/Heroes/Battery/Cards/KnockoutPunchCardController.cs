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
    public class KnockoutPunchCardController : CardController
    {
        public KnockoutPunchCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.ShowChargedStatus(SpecialStringMaker, CharacterCard);
        }

        public override IEnumerator Play()
        {
            // {BatteryCharacter} deals 1 target 2 lightning damage.
            var damageActions = new List<DealDamageAction>();
            var e = GameController.SelectTargetsAndDealDamage(
                HeroTurnTakerController,
                new DamageSource(GameController, CharacterCard),
                2,
                DamageType.Lightning,
                numberOfTargets: 1,
                optional: false,
                requiredTargets: 1,
                storedResultsDamage: damageActions,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            // If {BatteryCharacter} is {Charged}, a non-character target dealt damage this way loses
            // all 'start of turn' and 'end of turn' effects on its card until the start of your next turn."
            if (this.IsCharged(CharacterCard))
            {
                foreach (var dda in damageActions)
                {
                    if (!dda.DidDealDamage || !dda.Target.Is().Noncharacter().Target()) continue;

                    {
                        var effect = new PreventPhaseEffectStatusEffect(Phase.Start);
                        effect.UntilStartOfNextTurn(TurnTaker);
                        effect.CardCriteria.IsSpecificCard = dda.Target;

                        e = AddStatusEffect(effect);
                        if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                        else { GameController.ExhaustCoroutine(e); }
                    }

                    {
                        var effect = new PreventPhaseEffectStatusEffect(Phase.End);
                        effect.UntilStartOfNextTurn(TurnTaker);
                        effect.CardCriteria.IsSpecificCard = dda.Target;

                        e = AddStatusEffect(effect);
                        if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                        else { GameController.ExhaustCoroutine(e); }
                    }
                }
            }
        }
    }
}
