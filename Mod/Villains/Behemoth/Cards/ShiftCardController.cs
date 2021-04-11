﻿using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Behemoth
{
    public class ShiftCardController : BehemothUtilityCardController
    {
        public ShiftCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            // "Each player moves all proximity tokens from their hero to the hero of the player with an active hero immediately to their right."
            // Empty all proximity pools, store the removed values in turn order
            List<TokenPool> proximities = new List<TokenPool>();
            IEnumerable<HeroTurnTakerController> heroControllers = base.GameController.FindHeroTurnTakerControllers().Where((HeroTurnTakerController httc) => !httc.IsIncapacitatedOrOutOfGame && ProximityPool(httc.TurnTaker) != null);
            if (heroControllers.Count() <= 1 || AllProximityPoolsEmpty())
            {
                yield break;
            }
            int[] poolValues = new int[heroControllers.Count()];
            foreach (HeroTurnTakerController player in heroControllers)
            {
                TokenPool playerProximity = ProximityPool(player.TurnTaker);
                proximities.Add(playerProximity);
            }
            for (int i = 0; i < proximities.Count(); i++)
            {
                List<RemoveTokensFromPoolAction> removal = new List<RemoveTokensFromPoolAction>();
                IEnumerator emptyCoroutine = base.GameController.RemoveTokensFromPool(proximities.ElementAt(i), proximities.ElementAt(i).CurrentValue, storedResults: removal, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(emptyCoroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(emptyCoroutine);
                }
                if (DidRemoveTokens(removal))
                {
                    poolValues[i] = GetNumberOfTokensRemoved(removal);
                }
            }
            // Add tokens to each pool equal to the stored value for the previous one
            for (int i = 0; i < proximities.Count(); i++)
            {
                int numToAdd = poolValues[(i + poolValues.Length - 1) % poolValues.Length];
                IEnumerator fillCoroutine = base.GameController.AddTokensToPool(proximities.ElementAt(i), numToAdd, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(fillCoroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(fillCoroutine);
                }
            }
            yield break;
        }
    }
}
