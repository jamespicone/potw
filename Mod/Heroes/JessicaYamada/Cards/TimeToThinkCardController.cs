﻿
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.JessicaYamada
{
    class TimeToThinkCardController : CardController
    {
        public TimeToThinkCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            /*
                "Whenever a player plays no cards during their play phase, they may draw a card",
                "Whenever a player uses no powers during their power phase, they may draw a card",
                "Whenever a player skips a phase, they may draw a card"
            */
            AddPhaseChangeTrigger(
                tt => tt.IsHero,
                p => true,
                pca => 
                    pca.FromPhase.WasSkipped ||
                    (
                        (pca.FromPhase.PhaseActionCountUsed ?? 0) == 0 &&
                        (
                            pca.FromPhase.Phase == Phase.PlayCard ||
                            pca.FromPhase.Phase == Phase.UsePower
                        )
                    ),
                pca => DrawCardResponse(pca),
                new[] { TriggerType.DrawCard },
                TriggerTiming.Before
            );
        }

        public IEnumerator DrawCardResponse(PhaseChangeAction pca)
        {
            var htt = pca.FromPhase.TurnTaker as HeroTurnTaker;
            if (htt == null) { yield break; }

            var e = GameController.DrawCard(htt, optional: true, cardSource: GetCardSource());
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
