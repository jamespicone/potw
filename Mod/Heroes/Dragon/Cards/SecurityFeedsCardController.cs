using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Dragon
{
    public class SecurityFeedsCardController : CardController
    {
        public SecurityFeedsCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator ActivateAbilityEx(CardDefinition.ActivatableAbilityDefinition ability)
        {
            if (ability.Name != "focus") { yield break; }

            // Reveal the top card of the Environment deck. Either return it to the top of the deck or discard it
            var environment = FindEnvironment(TurnTaker.BattleZone);

            var e = RevealTopCard_PutItBackOrDiscardIt(HeroTurnTakerController, HeroTurnTakerController, environment.TurnTaker.Deck);
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
