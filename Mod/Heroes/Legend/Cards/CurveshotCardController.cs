using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Jp.ParahumansOfTheWormverse.Legend
{
    public class CurveshotCardController : CardController
    {
        public CurveshotCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            AddThisCardControllerToList(CardControllerListType.CanCauseDamageOutOfPlay);
        }

        public override void AddTriggers()
        {
            AddTrigger<DealDamageAction>(
                dda => dda.CardSource.Card == Card && dda.DamageSource.Card == CharacterCard,
                dda => DoCurveshot(dda),
                TriggerType.FirstTrigger,
                TriggerTiming.Before
            );

            AddTrigger<ImmuneToDamageAction>(
                itda => itda.DealDamageAction.CardSource.Card == Card && itda.DealDamageAction.DamageSource.Card == CharacterCard,
                itda => CancelResponse(itda),
                TriggerType.CancelAction,
                TriggerTiming.Before,
                outOfPlayTrigger: true
            );

            AddTrigger<DealDamageAction>(
                dda=> dda.CardSource.Card == Card &&
                    dda.DamageSource.Card == CharacterCard &&
                    GameController.PreviewMode &&
                    dda.DamageModifiers.Count(mdda => mdda is ImmuneToDamageAction) > 0 &&
                    ! dda.CanDealDamage,
                dda => FixupPreview(dda),
                TriggerType.MakeImmuneToDamage,
                TriggerTiming.Before,
                priority: TriggerPriority.Low,
                outOfPlayTrigger: true
            );
        }

        private IEnumerator FixupPreview(DealDamageAction dda)
        {
            if (dda.DamageModifiers.Count(mdda => mdda is ImmuneToDamageAction) <= 0 || ! GameController.PreviewMode)
            {
                yield break;
            }

            // Force triggermanager to see this as ambiguous outcome            
            var e = GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.AmbiguousDecision, Card, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            // This is super hacky and questionable but maybe it's okay because it's only for previews?
            dda.Amount = dda.OriginalAmount;
            dda.CanDealDamage = true;
            foreach (var md in dda.DamageModifiers.Where(mdda => !(mdda is ImmuneToDamageAction)))
            {
                md.ModifyDamage();
            }           
        }

        private IEnumerator DoCurveshot(DealDamageAction dda)
        {
            var e = GameController.MakeDamageNotRedirectable(dda, GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            e = GameController.MakeDamageIrreducible(dda, GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // Select a Target, then apply an Effect to it
            var targets = new List<Card>();
            var effects = new List<IEffectCardController>();

            var e = this.ChooseEffects(effects);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            var possibleTargets = FindCardsWhere(
                c => c.IsTarget && c.IsInPlay,
                realCardsOnly: true,
                visibleToCard: GetCardSource()
            );

            var damage = new List<DealDamageAction>();
            foreach (var effect in effects)
            {
                damage.Add(effect.TypicalDamageAction(targets));
            }

            var storedResult = new List<SelectCardDecision>();
            e = GameController.SelectCardAndStoreResults(
                HeroTurnTakerController,
                SelectionType.SelectTarget,
                possibleTargets,
                storedResult,
                dealDamageInfo: damage,
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

            var card = GetSelectedCard(storedResult);
            if (card == null) { yield break; }

            e = this.ApplyEffects(effects, new Card[] { card }, EffectTargetingOrdering.OrderingAlreadyDecided, GetCardSource());
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
