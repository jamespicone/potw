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
    public class KnockoutPunchCardController : BatteryUtilityCardController
    {
        public KnockoutPunchCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            ShowBatteryChargedStatus();
        }

        public override IEnumerator Play()
        {
            // "{BatteryCharacter} deals 1 target 2 lightning damage."
            List<DealDamageAction> damageActions = new List<DealDamageAction>();
            IEnumerator damageCoroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), 2, DamageType.Lightning, 1, false, 1, storedResultsDamage: damageActions, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(damageCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(damageCoroutine);
            }
            // "If {BatteryCharacter} is {Charged}, a non-character card target dealt damage this way loses all 'start of turn' and 'end of turn' effects on its card until the start of your next turn."
            if (IsBatteryCharged())
            {
                IEnumerable<Card> validChoices = (from dda in damageActions where dda.DidDealDamage && dda.Target != null && (! dda.Target.IsCharacter) select dda.Target).Distinct();
                Card chosen = validChoices.FirstOrDefault();
                if (validChoices.Count() > 0)
                {
                    List<SelectCardDecision> selected = new List<SelectCardDecision>();
                    IEnumerator selectCoroutine = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.None, new LinqCardCriteria((Card c) => validChoices.Contains(c)), selected, false, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(selectCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(selectCoroutine);
                    }
                    if (selected != null && selected.Count() > 0)
                    {
                        if (selected.FirstOrDefault() != null && selected.FirstOrDefault().SelectedCard != null)
                        {
                            chosen = selected.FirstOrDefault().SelectedCard;
                        }
                    }
                }
                if (chosen != null)
                {
                    PreventPhaseEffectStatusEffect preventStart = new PreventPhaseEffectStatusEffect(Phase.Start);
                    preventStart.UntilStartOfNextTurn(base.TurnTaker);
                    preventStart.CardCriteria.IsSpecificCard = chosen;
                    IEnumerator preventStartCoroutine = base.GameController.AddStatusEffect(preventStart, true, GetCardSource());
                    PreventPhaseEffectStatusEffect preventEnd = new PreventPhaseEffectStatusEffect(Phase.End);
                    preventEnd.UntilStartOfNextTurn(base.TurnTaker);
                    preventEnd.CardCriteria.IsSpecificCard = chosen;
                    IEnumerator preventEndCoroutine = base.GameController.AddStatusEffect(preventEnd, true, GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(preventStartCoroutine);
                        yield return base.GameController.StartCoroutine(preventEndCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(preventStartCoroutine);
                        base.GameController.ExhaustCoroutine(preventEndCoroutine);
                    }
                }
            }
            yield break;
        }
    }
}
