using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;
using Handelabra;

namespace Jp.ParahumansOfTheWormverse.Labyrinth
{
    public class DeviousLabyrinthCardController : ShapingCardController
    {
        public DeviousLabyrinthCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddShapingTriggers()
        {
            AddTargetEntersPlayTrigger(
                c => c.Is().Noncharacter().Target(),
                MaybeBlank,
                new TriggerType[] { TriggerType.InhibitCard, TriggerType.CreateStatusEffect },
                TriggerTiming.After
            );

            AddTrigger<TargetLeavesPlayAction>(
                tpa => WeAreBlanking(tpa.TargetLeavingPlay),
                UnblankTargetLeavingPlay,
                TriggerType.Hidden,
                TriggerTiming.Before
            );

            // TODO: workaround for turntaker replace silently expiring all status effects
            // leaving blanked stuff stuck.
            //
            // Can't trigger on gameover; that happens after the status effects are expired.
        }

        private bool WeAreBlanking(Card c)
        {
            var myStatusEffects = GameController.StatusEffectManager.StatusEffectControllers
                .Select(sec => sec.StatusEffect)
                .Where(se => se.CardSource == Card && se is OnPhaseChangeStatusEffect);
            foreach (var effect in myStatusEffects)
            {
                if (effect.TargetLeavesPlayExpiryCriteria.IsOneOfTheseCards.Contains(c))
                {
                    return true;
                }
            }

            return false;
        }

        private IEnumerator UnblankTargetLeavingPlay(TargetLeavesPlayAction tpa)
        {
            UnblankCard(tpa.TargetLeavingPlay);
            yield break;
        }

        private IEnumerator MaybeBlank(Card target)
        {
            // you may destroy an Environment card.
            var storedResults = new List<DestroyCardAction>();
            var e = GameController.SelectAndDestroyCard(
                HeroTurnTakerController,
                new LinqCardCriteria(c => c.Is().Environment(), "environment"),
                optional: true,
                storedResults,
                responsibleCard: CharacterCard,
                GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            // If you do...
            if (! DidDestroyCard(storedResults)) yield break;

            // that target has no game text until the start of your next turn
            var effect = new OnPhaseChangeStatusEffect(
                CardWithoutReplacements,
                nameof(CardBlankingExpired),
                $"{target.Title} has no game text until the start of {HeroTurnTaker.ShortName}'s next turn",
                new TriggerType[] { TriggerType.Hidden },
                Card
            );

            effect.UntilTargetLeavesPlay(target);
            effect.TurnPhaseCriteria.TurnTaker = TurnTaker;
            effect.TurnPhaseCriteria.Phase = Phase.Start;

            BlankCard(target);

            e = AddStatusEffect(effect);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }

        private void BlankCard(Card c)
        {
            c.SetIsBlank(true);
            GameController.AddInhibitor(FindCardController(c));
        }

        private void UnblankCard(Card c)
        {
            c.SetIsBlank(false);
            GameController.RemoveInhibitor(FindCardController(c));
        }

        public IEnumerator CardBlankingExpired(PhaseChangeAction pca, StatusEffect effect)
        {
            Log.Debug($"Calling CardBlankingExpired {effect}");

            var card = effect.TargetLeavesPlayExpiryCriteria.IsOneOfTheseCards.FirstOrDefault();
            if (card == null) { yield break; }

            UnblankCard(card);

            var e = GameController.ExpireStatusEffect(effect, GetCardSource());
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }
    }
}
