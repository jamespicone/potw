using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Dragon
{
    public class ArchivesCardController : CardController
    {
        public ArchivesCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator ActivateAbilityEx(CardDefinition.ActivatableAbilityDefinition ability)
        {
            if (ability.Name != "focus") { yield break; }

            // A player may take a One-Shot from their trash and put it into their hand
            var e = GameController.SelectHeroToMoveCardFromTrash(
                HeroTurnTakerController,
                httc => httc.HeroTurnTaker.Hand,
                cardCriteria: new LinqCardCriteria(c => c.IsOneShot, "one-shot"),
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
