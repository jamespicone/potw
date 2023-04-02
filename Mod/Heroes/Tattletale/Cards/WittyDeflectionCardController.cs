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
    public class WittyDeflectionCardController : CardController
    {
        public override bool AllowFastCoroutinesDuringPretend
        {
            get
            {
                if (Game.StatusEffects.Any((StatusEffect se) => se.CardSource == this.Card))
                {
                    return false;
                }
                return true;
            }
        }
        public WittyDeflectionCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            base.AddTriggers();
        }

        public override IEnumerator Play()
        {
            yield break;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // "Select a hero target."
            List<SelectCardDecision> choice = new List<SelectCardDecision>();
            IEnumerator chooseCoroutine = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.RedirectDamageDirectedAtTarget, new LinqCardCriteria((Card c) => c.IsInPlay && c.Is(this).Hero().Target(), "hero target", useCardsSuffix: false, useCardsPrefix: false, "hero target", "hero targets"), choice, false, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(chooseCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(chooseCoroutine);
            }
            SelectCardDecision chosen = choice.FirstOrDefault();
            if (chosen != null && chosen.SelectedCard != null)
            {
                Card chosenTarget = chosen.SelectedCard;
                // "Until the start of your next turn, whenever that target would be dealt damage, you may redirect that damage to another hero target."
                OnDealDamageStatusEffect redirectOptionStatus = new OnDealDamageStatusEffect(base.CardWithoutReplacements, nameof(OptionalRedirectResponse), "When " + chosenTarget.Title + " would be dealt damage, " + base.HeroTurnTakerController.Name + " may redirect that damage to another hero target.", new TriggerType[] { TriggerType.RedirectDamage, TriggerType.WouldBeDealtDamage }, base.TurnTaker, base.Card);
                // "Until the start of your next turn..."
                redirectOptionStatus.UntilStartOfNextTurn(base.TurnTaker);
                // "... whenever that target would be dealt damage..."
                redirectOptionStatus.TargetCriteria.IsSpecificCard = chosenTarget;
                redirectOptionStatus.DamageAmountCriteria.GreaterThan = 0;

                redirectOptionStatus.UntilTargetLeavesPlay(chosenTarget);
                redirectOptionStatus.BeforeOrAfter = BeforeOrAfter.Before;

                IEnumerator statusCoroutine = base.GameController.AddStatusEffect(redirectOptionStatus, true, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(statusCoroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(statusCoroutine);
                }
            }
            yield break;
        }

        public IEnumerator OptionalRedirectResponse(DealDamageAction dd, TurnTaker hero, StatusEffect effect, int[] powerNumerals = null)
        {
            // "... you may redirect that damage to another hero target."
            List<SelectCardDecision> selectTarget = new List<SelectCardDecision>();

            AddInhibitorException(ga => ga is RedirectDamageAction);
            IEnumerator selectCoroutine = base.GameController.SelectTargetAndRedirectDamage(base.HeroTurnTakerController, (Card c) => c.IsInPlay && c.Is(this).Hero().Target() && c != dd.Target, dd, optional: true, selectTarget, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(selectCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(selectCoroutine);
            }
            RemoveInhibitorException();
        }
    }
}
