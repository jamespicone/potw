using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Labyrinth
{
    public class DisassociationCardController : CardController
    {
        public DisassociationCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            AddThisCardControllerToList(CardControllerListType.AddsPowers);
            AddThisCardControllerToList(CardControllerListType.IncreasePhaseActionCount);
        }

        public override IEnumerable<Power> AskIfContributesPowersToCardController(CardController cardController)
        {
            var ret = new List<Power>();

            if (cardController.Is().Hero().Character().AccordingTo(this) && cardController != CharacterCardController)
            {
                ret.Add(new Power(
                    HeroTurnTakerController,
                    cardController,
                    "Destroy {Disassociation}.",
                    () => DestroyThisCardResponse(null),
                    0,
                    null,
                    cardSource: GetCardSource()
                ));
            }

            return ret;
        }

        public override void AddTriggers()
        {
            // {LabyrinthCharacter} is immune to psychic damage.
            AddImmuneToDamageTrigger(dda => dda.Target == CharacterCard && dda.DamageType == DamageType.Psychic);

            // At the end of {LabyrinthCharacter}'s turn {LabyrinthCharacter} deals herself 2 irreducible melee damage,
            // then may play a card
            AddEndOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => DamageAndCardPlay(),
                new TriggerType[] { TriggerType.DealDamage, TriggerType.PlayCard }
            );
        }

        private IEnumerator DamageAndCardPlay()
        {
            var e = DealDamage(CharacterCard, CharacterCard, 2, DamageType.Melee, isIrreducible: true, cardSource: GetCardSource());
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            e = SelectAndPlayCardFromHand(HeroTurnTakerController);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }

        public override IEnumerator UsePower(int index)
        {
            // Put a Shaping card into play with the top card of the Environment deck under it.
            var storedCard = new List<SelectCardDecision>();
            var e = GameController.SelectCardAndStoreResults(
                HeroTurnTakerController,
                SelectionType.PutIntoPlay,
                new LinqCardCriteria(c => c.Is().WithKeyword("shaping").AccordingTo(this) && c.IsInLocation(HeroTurnTaker.Hand), "shaping"),
                storedCard,
                optional: false,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            var cardToPlay = GetSelectedCard(storedCard);
            if (cardToPlay == null) { yield break; }

            var envdeck = FindEnvironment().TurnTaker.Deck;
            if (envdeck.HasCards)
            {
                e = GameController.MoveCard(
                    TurnTakerController,
                    envdeck.TopCard,
                    cardToPlay.UnderLocation,
                    cardSource: GetCardSource()
                );
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }

            var wasPlayed = new List<bool>();
            e = GameController.PlayCard(
                TurnTakerController,
                cardToPlay,
                isPutIntoPlay: true,
                wasCardPlayed: wasPlayed,
                overridePlayLocation: envdeck.OwnerTurnTaker.PlayArea,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            if (! wasPlayed.FirstOrDefault())
            {
                // Card wasn't played for some reason; get rid of the undercard.
                e = GameController.BulkMoveCards(
                    TurnTakerController,
                    cardToPlay.UnderLocation.Cards,
                    envdeck.OwnerCard.NativeTrash,
                    cardSource: GetCardSource()
                );
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }
        }
    }
}
