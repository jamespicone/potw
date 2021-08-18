using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Alexandria
{
    public class ImperiousDemeanourCardController : CardController
    {
        public ImperiousDemeanourCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            // "When this card enters play draw a card",
            var e = DrawCard(HeroTurnTaker);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        public override void AddTriggers()
        {
            // "Whenever you use a power {AlexandriaCharacter} regains 3 HP"
            AddTrigger<UsePowerAction>(
                upa => upa.HeroUsingPower == HeroTurnTakerController && upa.IsSuccessful,
                upa => HealUp(),
                TriggerType.GainHP,
                TriggerTiming.Before
            );
        }

        private IEnumerator HealUp()
        {
            var selectedCard = new List<Card>();
            var e = FindCharacterCard(
                TurnTaker,
                SelectionType.GainHP,
                selectedCard
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            var card = selectedCard.FirstOrDefault();
            if (card == null) { yield break; }

            e = GameController.GainHP(card, 3, cardSource: GetCardSource());
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
