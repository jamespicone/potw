using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Dragon
{
    public class ServerFarmCardController : CardController
    {
        public ServerFarmCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        // "At the start of your turn gain a Focus point"
        public override void AddTriggers()
        {
            AddStartOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => CharacterCardController.AddFocusTokens(1, GetCardSource()),
                TriggerType.AddTokensToPool
            );
        }

        public override IEnumerator ActivateAbilityEx(CardDefinition.ActivatableAbilityDefinition ability)
        {
            if (ability.Name != "focus") { yield break; }

            // Destroy this card. When it goes to the trash, gain 3 focus points
            var e = GameController.DestroyCard(
                HeroTurnTakerController,
                Card,
                responsibleCard: Card,
                postDestroyAction: () => CharacterCardController.AddFocusTokens(3, GetCardSource()),
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
