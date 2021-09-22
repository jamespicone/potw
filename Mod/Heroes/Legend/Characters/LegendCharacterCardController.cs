using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Handelabra;

namespace Jp.ParahumansOfTheWormverse.Legend
{

    public class LegendCharacterCardController : HeroCharacterCardController, IEffectCardController
    {
        public LegendCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            yield break;
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

            e = this.ApplyEffects(effects, new Card[] { card }, EffectTargetingOrdering.OrderingAlreadyDecided);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        public DealDamageAction TypicalDamageAction(IEnumerable<Card> targets)
        {
            return new DealDamageAction(GetCardSource(), new DamageSource(GameController, CharacterCard), null, 2, DamageType.Energy);
        }

        public IEnumerator DoEffect(IEnumerable<Card> targets, EffectTargetingOrdering ordering)
        {
            // "Legend deals 2 energy damage"
            foreach (var c in targets)
            {
                var e = DealDamage(
                    CharacterCard,
                    c,
                    2,
                    DamageType.Energy,
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
}
