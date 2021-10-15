using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.Dauntless
{
    public class HeroicImageCardController : CardController
    {
        public HeroicImageCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            // "{DauntlessCharacter} regains 2 HP",
            var e = GameController.GainHP(CharacterCard, 2, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            //  "Select a hero target. Until the start of your next turn whenever that target would be dealt damage redirect that damage to {DauntlessCharacter}"
            var choice = new List<SelectCardDecision>();
            e = GameController.SelectCardAndStoreResults(
                HeroTurnTakerController,
                SelectionType.RedirectDamageDirectedAtTarget,
                new LinqCardCriteria(c => c.IsInPlay && this.HasAlignment(c, CardAlignment.Hero, CardTarget.Target) && c != CharacterCard, "hero target", useCardsSuffix: false, useCardsPrefix: false, "hero target", "hero targets"),
                choice,
                optional: false,
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

            var selectedCard = GetSelectedCard(choice);
            if (selectedCard != null)
            {
                // "Until the start of your next turn, whenever that target would be dealt damage, you may redirect that damage to another hero target."
                var redirectStatus = new OnDealDamageStatusEffect(
                    CardWithoutReplacements,
                    nameof(RedirectResponse),
                    "When " + selectedCard.Title + " would be dealt damage, it is redirected to " + HeroTurnTakerController.Name,
                    new TriggerType[] { TriggerType.RedirectDamage, TriggerType.WouldBeDealtDamage },
                    TurnTaker,
                    Card
                );

                // "Until the start of your next turn..."
                redirectStatus.UntilStartOfNextTurn(base.TurnTaker);

                // "... whenever that target would be dealt damage..."
                redirectStatus.TargetCriteria.IsSpecificCard = selectedCard;
                redirectStatus.DamageAmountCriteria.GreaterThan = 0;

                redirectStatus.UntilTargetLeavesPlay(selectedCard);
                redirectStatus.BeforeOrAfter = BeforeOrAfter.Before;

                e = GameController.AddStatusEffect(redirectStatus, showMessage: true, GetCardSource());
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
