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

        public DealDamageAction TypicalDamageAction(IEnumerable<Card> targets)
        {
            var ret = new DealDamageAction(
                GetCardSource(),
                new DamageSource(GameController, CharacterCard),
                null,
                2,
                DamageType.Energy
            );

            ret.UnknownDamageType = true;
            return ret;
        }

        public IEnumerator DoEffect(IEnumerable<Card> targets, CardSource cardSource, EffectTargetingOrdering ordering)
        {
            // Select a damage type. Legend deals 2 damage of that type
            return this.HandleEffectOrdering(
                targets,
                ordering,
                t => SingleTargetEffect(t, cardSource),
                ts => MultiTargetEffect(ts, cardSource)
            );
        }

        private IEnumerator SingleTargetEffect(Card target, CardSource cardSource)
        {
            var dda = TypicalDamageAction(new Card[] { target });
            dda.Target = target;

            var storedResults = new List<SelectDamageTypeDecision>();
            var e = GameController.SelectDamageType(
                HeroTurnTakerController,
                storedResults,
                gameAction: dda,
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

            var damageType = GetSelectedDamageType(storedResults);
            if (damageType == null) { yield break; }

            e = DealDamage(
                CharacterCard,
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

        private IEnumerator MultiTargetEffect(IEnumerable<Card> targets, CardSource cardSource)
        {
            var dda = TypicalDamageAction(targets);

            // TODO: This doesn't work
            SelectTargetsDecision decision = new SelectTargetsDecision(
                GameController,
                HeroTurnTakerController,
                c => targets.Contains(c),
                allowAutoDecide: true,
                numberOfCards: targets.Count(),
                requiredDecisions: targets.Count(),
                cardSource: GetCardSource(),
                damageSource: dda.DamageSource,
                amount: dda.Amount,
                selectTargetsEvenIfCannotPerformAction: true
            );

            var e = GameController.SelectCardsAndDoAction(
                decision,
                scd => SingleTargetEffect(scd.SelectedCard, cardSource),
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
