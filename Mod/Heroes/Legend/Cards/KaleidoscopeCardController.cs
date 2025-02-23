using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Legend
{
    public class KaleidoscopeCardController : CardController, IEffectCardController
    {
        public KaleidoscopeCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public DealDamageAction TypicalDamageAction(IEnumerable<Card> targets, CardController sourceCard, CardSource cardSource)
        {
            var ret = new DealDamageAction(
                cardSource,
                new DamageSource(GameController, sourceCard.CharacterCard),
                null,
                2,
                DamageType.Energy
            );

            ret.UnknownDamageType = true;
            return ret;
        }

        public IEnumerator DoEffect(IEnumerable<Card> targets, CardController sourceCard, CardSource cardSource, EffectTargetingOrdering ordering)
        {
            // Select a damage type. Legend deals 2 damage of that type
            return this.HandleEffectOrdering(
                targets,
                ordering,
                t => SingleTargetEffect(t, sourceCard, cardSource),
                ts => MultiTargetEffect(ts, sourceCard, cardSource)
            );
        }

        private IEnumerator SingleTargetEffect(Card target, CardController sourceCard, CardSource cardSource)
        {
            var dda = TypicalDamageAction(new Card[] { target }, sourceCard, cardSource);
            dda.Target = target;

            var storedResults = new List<SelectDamageTypeDecision>();
            var e = GameController.SelectDamageType(
                sourceCard.HeroTurnTakerController,
                storedResults,
                gameAction: dda,
                cardSource: cardSource
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            var damageType = GetSelectedDamageType(storedResults);
            if (damageType == null) { yield break; }

            e = DealDamage(
                sourceCard.CharacterCard,
                target,
                dda.Amount,
                damageType.Value,
                cardSource: cardSource
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

        private IEnumerator MultiTargetEffect(IEnumerable<Card> targets, CardController sourceCard, CardSource cardSource)
        {
            var dda = TypicalDamageAction(targets, sourceCard, cardSource);

            // TODO: This doesn't work
            SelectTargetsDecision decision = new SelectTargetsDecision(
                GameController,
                sourceCard.HeroTurnTakerController,
                c => targets.Contains(c),
                allowAutoDecide: true,
                numberOfCards: targets.Count(),
                requiredDecisions: targets.Count(),
                cardSource: cardSource,
                damageSource: dda.DamageSource,
                amount: dda.Amount,
                selectTargetsEvenIfCannotPerformAction: true
            );

            var e = GameController.SelectCardsAndDoAction(
                decision,
                scd => SingleTargetEffect(scd.SelectedCard, sourceCard, cardSource),
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
