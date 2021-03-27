using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Armsmaster
{
    public class StasisEffectorCardController : ModuleCardController
    {
        public StasisEffectorCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator DoPrimary()
        {
            // Discard 2 cards. If you do, the next time a villain card would be played prevent it; and until the start of your next turn all villain character cards are immune to damage
            var discardResults = new List<DiscardCardAction>();
            var e = SelectAndDiscardCards(
                HeroTurnTakerController,
                2,
                storedResults: discardResults,
                responsibleTurnTaker: TurnTaker
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            if (GetNumberOfCardsDiscarded(discardResults) < 2) { yield break; }

            var cannotPlayCardsEffect = new CannotPlayCardsStatusEffect();
            // TODO: This doesn't work. CannotPlayCards status effects don't subtract from NumberOfUses
            // Might have to change the wording.
            cannotPlayCardsEffect.NumberOfUses = 1;
            cannotPlayCardsEffect.TurnTakerCriteria.IsVillain = true;
            cannotPlayCardsEffect.CardSource = Card;

            e = AddStatusEffect(cannotPlayCardsEffect);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            var villainDamageImmunityEffect = new ImmuneToDamageStatusEffect();
            villainDamageImmunityEffect.UntilStartOfNextTurn(TurnTaker);
            villainDamageImmunityEffect.TargetCriteria.IsVillain = true;
            villainDamageImmunityEffect.TargetCriteria.IsCharacter = true;
            villainDamageImmunityEffect.CardSource = Card;

            e = AddStatusEffect(villainDamageImmunityEffect);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        public override IEnumerator DoSecondary()
        {
            // Destroy a target with 4 or less HP
            var e = GameController.SelectAndDestroyCard(HeroTurnTakerController, new LinqCardCriteria(c => c.IsTarget && c.HitPoints <= 4), optional: false, responsibleCard: Card, cardSource: GetCardSource());
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
