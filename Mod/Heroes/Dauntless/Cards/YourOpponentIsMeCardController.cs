using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Dauntless
{
    public class YourOpponentIsMeCardController : CardController
    {
        public YourOpponentIsMeCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            // "{DauntlessCharacter} deals 2 energy damage to up to 2 targets.",
            var e = GameController.SelectTargetsAndDealDamage(
                HeroTurnTakerController,
                new DamageSource(GameController, CharacterCard),
                amount: 2,
                DamageType.Energy,
                numberOfTargets: 2,
                optional: false,
                requiredTargets: 0,
                addStatusEffect: dda => AddRedirect(dda),
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

        private IEnumerator AddRedirect(DealDamageAction dda)
        {
            if (! dda.DidDealDamage) { yield break; }

            // "Until the start of your next turn whenever those targets would deal damage redirect that damage to {DauntlessCharacter}"
            var redirectStatus = new OnDealDamageStatusEffect(
                CardWithoutReplacements,
                nameof(RedirectResponse),
                "When " + dda.Target.Title + " would deal damage, it is redirected to " + HeroTurnTakerController.Name,
                new TriggerType[] { TriggerType.RedirectDamage, TriggerType.DealDamage },
                TurnTaker,
                Card
            );

            // "Until the start of your next turn..."
            redirectStatus.UntilStartOfNextTurn(TurnTaker);

            // "... whenever that target would deal damage..."
            redirectStatus.SourceCriteria.IsSpecificCard = dda.Target;
            redirectStatus.DamageAmountCriteria.GreaterThan = 0;

            redirectStatus.UntilTargetLeavesPlay(dda.Target);
            redirectStatus.BeforeOrAfter = BeforeOrAfter.Before;

            var e = AddStatusEffect(redirectStatus);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        public IEnumerator RedirectResponse(DealDamageAction dd, TurnTaker hero, StatusEffect effect, int[] powerNumerals = null)
        {
            //  ...redirect that damage to {DauntlessCharacter}
            var e = GameController.RedirectDamage(
                dd,
                CharacterCard,
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
