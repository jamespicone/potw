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
    public class InformationOverloadCardController : CardController
    {
        public InformationOverloadCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            // "Whenever {TattletaleCharacter} uses a power this turn, she deals herself 1 psychic damage."
            DealDamageAfterUsePowerStatusEffect backlashStatus = new DealDamageAfterUsePowerStatusEffect(base.HeroTurnTaker, base.CharacterCard, base.CharacterCard, 1, DamageType.Psychic, 1, false);
            backlashStatus.UntilThisTurnIsOver(base.Game);
            IEnumerator statusCoroutine = base.AddStatusEffect(backlashStatus);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(statusCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(statusCoroutine);
            }

            // "{TattletaleCharacter} may use any number of powers this turn."
            IEnumerator powersCoroutine = AdditionalPhaseActionThisTurn(base.TurnTaker, Phase.UsePower, 9999);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(powersCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(powersCoroutine);
            }
            
            yield break;
        }
    }
}
