using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Dragon
{
    public class DragonsROMCardController : CardController
    {
        public DragonsROMCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator ActivateAbilityEx(CardDefinition.ActivatableAbilityDefinition ability)
        {
            if (ability.Name == "focus")
            {
                IEnumerator e;
                switch (ability.Number)
                {
                    case 0:
                        // Draw a card
                        e = DrawCard(HeroTurnTaker);
                        break;

                    case 1:
                        // Play a card
                        e = SelectAndPlayCardFromHand(HeroTurnTakerController);
                        break;

                    case 2:
                        // Return a Mech in your play area to your hand
                        e = ReturnAMech();
                        break;

                    default:
                        yield break;
                }

                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }
            }
            else
            {
                yield return base.ActivateAbilityEx(ability);
            }
        }

        private IEnumerator ReturnAMech()
        {
            var stored = new List<SelectCardDecision>();
            var e = GameController.SelectCardAndStoreResults(
                HeroTurnTakerController,
                SelectionType.ReturnToHand,
                new LinqCardCriteria(c => c.DoKeywordsContain("mech") && c.IsAtLocationRecursive(TurnTaker.PlayArea), "Mech in your play area"),
                stored,
                optional: false,
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

            var card = GetSelectedCard(stored);
            if (card == null) { yield break; }

            e = GameController.MoveCard(
                TurnTakerController,
                card,
                HeroTurnTaker.Hand,
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
