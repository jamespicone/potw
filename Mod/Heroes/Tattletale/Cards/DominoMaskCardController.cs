using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Tattletale
{
    public class DominoMaskCardController : CardController
    {
        public override bool DoesHaveActivePlayMethod => false;
        public DominoMaskCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            base.GameController.AddCardControllerToList(CardControllerListType.IncreasePhaseActionCount, this);
        }

        public override void AddTriggers()
        {
            AddAdditionalPhaseActionTrigger((TurnTaker tt) => tt == base.TurnTaker, Phase.UsePower, 1);
            base.AddTriggers();
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine = IncreasePhaseActionCountIfInPhase((TurnTaker tt) => tt == base.TurnTaker, Phase.UsePower, 1);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override bool AskIfIncreasingCurrentPhaseActionCount()
        {
            if (base.GameController.ActiveTurnPhase.IsUsePower)
            {
                return base.GameController.ActiveTurnTaker == base.TurnTaker;
            }
            return false;
        }
    }
}
