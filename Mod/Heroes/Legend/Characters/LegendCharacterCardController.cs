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
            var storedResult = new List<SelectCardDecision>();
            var e = GameController.SelectCardAndStoreResults(
                HeroTurnTakerController,
                SelectionType.SelectTarget,
                new LinqCardCriteria(c => c.IsTarget && c.IsInPlay, "target"),
                storedResult,
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

            e = this.SelectAndPerformEffects(new Card[] { card });
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        public IEnumerator DoEffect(IEnumerable<Card> targets)
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
