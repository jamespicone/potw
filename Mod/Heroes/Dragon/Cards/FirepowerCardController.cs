using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Jp.ParahumansOfTheWormverse.Dragon
{
    public class FirepowerCardController : CardController
    {
        public FirepowerCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            var storedTargets = new List<SelectTargetDecision>();
            // Choose either Dragon or a Mech in this play area. They deal a target 3 fire damage
            var e = GameController.SelectTargetAndStoreResults(
                HeroTurnTakerController,
                FindCardsWhere(new LinqCardCriteria(c => c == CharacterCard || (c.DoKeywordsContain("mech") && TurnTaker.PlayArea == c.Location))),
                storedTargets,
                selectionType: SelectionType.CardToDealDamage,
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

            if (storedTargets.Count <= 0) { yield break; }
            var selectedTarget = storedTargets.First().SelectedCard;
            if (selectedTarget == null) { yield break; }

            e = GameController.SelectTargetsAndDealDamage(
                HeroTurnTakerController,
                new DamageSource(GameController, selectedTarget),
                3,
                DamageType.Fire,
                1,
                optional: false,
                requiredTargets: 1,
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
