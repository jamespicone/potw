
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.JessicaYamada
{
    class ComfortableOfficeCardController : CardController
    {
        public ComfortableOfficeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // At the start of your turn, a player regains 3 HP
            AddStartOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => HealSomeone(),
                TriggerType.DrawCard
            );
        }

        public IEnumerator HealSomeone()
        {
            var e = GameController.SelectAndGainHP(
                HeroTurnTakerController,
                3,
                additionalCriteria: c => c.Is().Hero().Target().Character(),
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
