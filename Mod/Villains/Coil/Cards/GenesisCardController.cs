using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

using UnityEngine;

namespace Jp.ParahumansOfTheWormverse.Coil
{
    public class GenesisCardController : CardController
    {
        public GenesisCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"When this card is destroyed shuffle it back into the villain deck.",
            // AddWhenDestroyedTrigger(dca => ShuffleBack(dca), TriggerType.ShuffleCardIntoDeck);

            //"At the end of the villain turn this card deals 2 poison damage to {H} hero targets."
            AddDealDamageAtEndOfTurnTrigger(
                TurnTaker,
                Card,
                c => c.IsHeroTarget(),
                TargetType.SelectTarget,
                2,
                DamageType.Toxic,
                numberOfTargets: H
            );
        }

        public override bool CanBeDestroyed => false;

        public override IEnumerator DestroyAttempted(DestroyCardAction dca)
        {
            var storedResults = new List<SelectLocationDecision>();
            var e = FindVillainDeck(
                DecisionMaker,
                SelectionType.ShuffleCardIntoDeck,
                storedResults,
                l => l.IsVillain
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            var villainDeck = GetSelectedLocation(storedResults);
            if (villainDeck == null) { yield break; }

            e = GameController.ShuffleCardIntoLocation(
                DecisionMaker,
                Card,
                villainDeck,
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
        }
    }
}
