﻿using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Behemoth
{
    public class DischargeCardController : BehemothUtilityCardController
    {
        public DischargeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            // "Change {BehemothCharacter}'s damage type to energy."
            IEnumerator energyCoroutine = SetBehemothDamageType(base.Card, DamageType.Energy);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(energyCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(energyCoroutine);
            }
            // "{BehemothCharacter} deals each hero target 2 damage."
            IEnumerator damageCoroutine = base.GameController.DealDamage(DecisionMaker, base.CharacterCard, (Card c) => c.Is(this).Hero().Target(), 2, GetBehemothDamageType(), cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(damageCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(damageCoroutine);
            }
            // "Each player may move 1 proximity token from their hero to another active hero."
            IEnumerator selectCoroutine = base.GameController.SelectTurnTakersAndDoAction(DecisionMaker, new LinqTurnTakerCriteria((TurnTaker tt) => tt.Is(this).Hero() && !tt.IsIncapacitatedOrOutOfGame), SelectionType.RemoveTokens, MayMoveOneTokenResponse, requiredDecisions: 0, allowAutoDecide: true, numberOfCards: 1, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(selectCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(selectCoroutine);
            }
            yield break;
        }
    }
}
