using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Jp.SOTMUtilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Alexandria
{
    public class MartialArtsCardController : CardController
    {
        public MartialArtsCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            // "When this card enters play you may destroy a non-character-card target with 5 or less HP",
            var e = GameController.SelectAndDestroyCard(
                HeroTurnTakerController,
                new LinqCardCriteria(c => ! c.IsCharacter && c.IsTarget && c.HitPoints <= 5, "noncharacter target with 5 or less HP"),
                optional: true,
                responsibleCard: CharacterCard,
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

        public override void AddTriggers()
        {
            // "When {AlexandriaCharacter} destroys a target select a target. Until the start of your next turn that target cannot deal damage"
            AddTrigger<DestroyCardAction>(
                dca => TurnTaker.IsResponsible(dca) && dca.WasCardDestroyed && dca.CardToDestroy.Card.IsTarget,
                dca => PreventTargetDoingDamage(),
                TriggerType.Other,
                TriggerTiming.After
            );
        }

        private IEnumerator PreventTargetDoingDamage()
        {
            // select a target. Until the start of your next turn that target cannot deal damage
            var storedResults = new List<SelectCardDecision>();
            var e = GameController.SelectCardAndStoreResults(
                HeroTurnTakerController,
                SelectionType.SelectTargetNoDamage,
                new LinqCardCriteria(c => c.IsTarget && c.IsInPlayAndHasGameText && ! c.IsBeingDestroyed, "target"),
                storedResults,
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

            var target = GetSelectedCard(storedResults);
            if (target == null) { yield break; }

            var effect = new CannotDealDamageStatusEffect();
            effect.SourceCriteria.IsSpecificCard = target;
            effect.UntilStartOfNextTurn(TurnTaker);

            e = GameController.AddStatusEffect(effect, showMessage: true, cardSource: GetCardSource());
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
