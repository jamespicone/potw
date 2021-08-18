using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Dragon
{
    public class PRTDatabaseCardController : CardController
    {
        public PRTDatabaseCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator ActivateAbilityEx(CardDefinition.ActivatableAbilityDefinition ability)
        {
            if (ability.Name != "focus") { yield break; }

            var storedVillain = new List<SelectLocationDecision>();
            var e = FindVillainDeck(HeroTurnTakerController, SelectionType.RevealTopCardOfDeck, storedVillain, l => true);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            var villain = GetSelectedLocation(storedVillain);
            if (villain == null) { yield break; }

            // Reveal the top card of the Villain deck. Either return it to the top of the deck or discard it
            e = RevealTopCard_PutItBackOrDiscardIt(HeroTurnTakerController, HeroTurnTakerController, villain);
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
