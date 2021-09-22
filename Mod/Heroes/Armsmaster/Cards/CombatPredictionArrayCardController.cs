using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Armsmaster
{
    public class CombatPredictionArrayCardController : ModuleCardController
    {
        public CombatPredictionArrayCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator DoPrimary()
        {
            // Reveal the top 3 cards of the villain deck and put them back in any order
            var storedLocation = new List<SelectLocationDecision>();
            var e = FindVillainDeck(HeroTurnTakerController, SelectionType.RevealCardsFromDeck, storedLocation, l => l.IsVillain);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            var selectedLocation = GetSelectedLocation(storedLocation);
            if (selectedLocation == null) { yield break; }

            e = RevealTheTopCardsOfDeck_MoveInAnyOrder(
                HeroTurnTakerController,
                TurnTakerController,
                selectedLocation.OwnerTurnTaker,
                3
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            e = CleanupRevealedCards(selectedLocation.OwnerTurnTaker.Revealed, selectedLocation);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        public override IEnumerator DoSecondary()
        {
            // Until the start of your next turn reduce damage dealt to Armsmaster by villain cards by 1
            ReduceDamageStatusEffect status = new ReduceDamageStatusEffect(1);
            status.SourceCriteria.IsVillain = true;
            status.TargetCriteria.IsSpecificCard = CharacterCard;
            status.UntilStartOfNextTurn(TurnTaker);

            var e = AddStatusEffect(status);
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
