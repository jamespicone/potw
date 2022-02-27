using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Tattletale
{
    public class InformationOverloadCardController : CardController
    {
        public InformationOverloadCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddTrigger<UsePowerAction>(
                upa => ShouldUpdatePhaseCount(),
                upa => IncreasePhaseActionCountIfInPhase(tt => tt == TurnTaker, Phase.UsePower, 10),
                TriggerType.IncreasePhaseActionCount,
                TriggerTiming.Before
            );
        }

        private bool ShouldUpdatePhaseCount()
        {
            var myStatusEffects = GameController.StatusEffectManager.StatusEffectControllers.Select(sec => sec.StatusEffect).Where(se => se.CardSource == Card);
            foreach (var effect in myStatusEffects)
            {
                var turntaker = effect.FromTurnPhaseExpiryCriteria.TurnTaker;
                var phase = FindTurnPhase(turntaker, Phase.UsePower);
                if (phase == null)
                {
                    continue;
                }

                if (Game.ActiveTurnPhase == phase)
                {
                    return (phase.GetPhaseActionCount() ?? 0) <= (phase.PhaseActionCountUsed ?? 0);
                }
            }

            return false;
        }

        public override IEnumerator Play()
        {
            IEnumerator e;

            // "Whenever {TattletaleCharacter} uses a power this turn, she deals herself 1 psychic damage."
            DealDamageAfterUsePowerStatusEffect backlashStatus = new DealDamageAfterUsePowerStatusEffect(HeroTurnTaker, CharacterCard, CharacterCard, 1, DamageType.Psychic, 1, false);
            backlashStatus.UntilThisTurnIsOver(Game);
            IEnumerator statusCoroutine = AddStatusEffect(backlashStatus);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(statusCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(statusCoroutine);
            }

            // "{TattletaleCharacter} may use any number of powers this turn."
            if (TurnTakerController.IsTurnTakersTurnPriorToOrDuringPhase(Phase.UsePower))
            {
                // It's either TT's turn during the power step or it's TT's turn and she's going to get her power step.
                // Give TT a bunch of power uses + a marker status effect that gives her more power uses when she runs out.
                OnPhaseChangeStatusEffect effect = new OnPhaseChangeStatusEffect(
                    CardWithoutReplacements,
                    "ExpireEffect",
                    $"{TurnTaker.NameRespectingVariant} may use any number of powers this turn",
                    new TriggerType[] { TriggerType.ModifyStatusEffect },
                    Card
                );

                effect.UntilThisTurnIsOver(Game);

                effect.TurnPhaseCriteria.Phase = Phase.UsePower;
                effect.TurnPhaseCriteria.TurnTaker = TurnTaker;

                e = AddStatusEffect(effect);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }

                e = AdditionalPhaseActionThisTurn(TurnTaker, Phase.UsePower, 100);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }
            }
            else
            {
                // It's either not TT's turn or she's had her power phase already. Just do an arbitrary number of powers.
                while (true)
                {
                    var powerResults = new List<UsePowerDecision>();
                    e = SelectAndUsePower(this, storedResults: powerResults);
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(statusCoroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(statusCoroutine);
                    }

                    if (powerResults.Where(pd => pd.SelectedPower != null).Count() <= 0)
                    {
                        break;
                    }
                }
            }
        }
    }
}
