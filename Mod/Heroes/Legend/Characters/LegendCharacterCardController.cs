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
            IEnumerator e;
            switch(index)
            {
                //"Legend deals 1 energy damage to a target",
                case 0:
                    e = GameController.SelectTargetsAndDealDamage(
                        HeroTurnTakerController,
                        new DamageSource(GameController, TurnTaker),
                        amount: 1,
                        DamageType.Energy,
                        numberOfTargets: 1,
                        optional: false,
                        requiredTargets: 1,
                        cardSource: GetCardSource()
                    );
                    break;

                //"A player may use a power",
                case 1:
                    e = GameController.SelectHeroToPlayCard(HeroTurnTakerController, cardSource: GetCardSource());
                    break;

                //"A player may draw a card"
                case 2:
                    e = GameController.SelectHeroToDrawCard(HeroTurnTakerController, cardSource: GetCardSource());
                    break;

                default:
                    yield break;
            }

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

        public DealDamageAction TypicalDamageAction(IEnumerable<Card> targets)
        {
            return new DealDamageAction(GetCardSource(), new DamageSource(GameController, CharacterCard), null, 2, DamageType.Energy);
        }

        public IEnumerator DoEffect(IEnumerable<Card> targets, CardSource cardSource, EffectTargetingOrdering ordering)
        {
            // "Legend deals 2 energy damage"
            return this.HandleEffectOrdering(
                 targets,
                 ordering,
                 t => GameController.DealDamageToTarget(
                     new DamageSource(GameController, CharacterCard),
                     t,
                     2,
                     DamageType.Energy,
                     cardSource: cardSource
                 ),
                 ts => GameController.DealDamage(
                     HeroTurnTakerController,
                     CharacterCard,
                     c => ts.Contains(c),
                     2,
                     DamageType.Energy,
                     cardSource: cardSource
                 )
             );
        }
    }
}
