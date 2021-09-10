using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Dragon
{
    public class KamikazeCardController : CardController
    {
        public KamikazeCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            //    "Select a Mech in this play area and a target.",
            //"If that target is a character card, the Mech deals the target 3 energy, fire, and melee damage.",
            //"If that target is not a character card, destroy it.",
            //"Destroy the Mech"
            var storedMech = new List<SelectCardDecision>();
            var e = GameController.SelectCardAndStoreResults(
                HeroTurnTakerController,
                SelectionType.CardToDealDamage,
                new LinqCardCriteria(c => c.DoKeywordsContain("mech") && c.Location == TurnTaker.PlayArea),
                storedMech,
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

            var selectedMech = GetSelectedCard(storedMech);
            if (selectedMech == null) { yield break; }

            var storedTarget = new List<SelectCardDecision>();
            e = GameController.SelectCardAndStoreResults(
                HeroTurnTakerController,
                SelectionType.SelectTarget,
                new LinqCardCriteria(c => c.IsInPlay && c.IsTarget),
                storedTarget,
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

            var selectedTarget = GetSelectedCard(storedTarget);
            if (selectedTarget == null) { yield break; }

            if (selectedTarget.IsCharacter)
            {
                var d1 = DealDamage(selectedMech, selectedTarget, 3, DamageType.Energy, cardSource: GetCardSource());
                var d2 = DealDamage(selectedMech, selectedTarget, 3, DamageType.Fire, cardSource: GetCardSource());
                var d3 = DealDamage(selectedMech, selectedTarget, 3, DamageType.Melee, cardSource: GetCardSource());

                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(d1);
                    yield return GameController.StartCoroutine(d2);
                    yield return GameController.StartCoroutine(d3);
                }
                else
                {
                    GameController.ExhaustCoroutine(d1);
                    GameController.ExhaustCoroutine(d2);
                    GameController.ExhaustCoroutine(d3);
                }
            }
            else
            {
                e = GameController.DestroyCard(HeroTurnTakerController, selectedTarget, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }
            }

            e = GameController.DestroyCard(HeroTurnTakerController, selectedMech, cardSource: GetCardSource());
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
