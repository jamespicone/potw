using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Alexandria
{
    public class TimeLockedCardController : CardController
    {
        public TimeLockedCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowIfElseSpecialString(
                HasRecordedHP,
                () => $"{RecordedHP()} is the value noted down",
                () => $"No value is noted down",
                HasRecordedHP
            );
        }

        private string HPProperty = "TimeLockedHP";

        public override IEnumerator Play()
        {
            //"Note down Alexandria's current HP",
            SetCardProperty(HPProperty, CharacterCard.HitPoints ?? -1);

            if (CharacterCard.HitPoints == null) { yield break; }

            var effect = new OnPhaseChangeStatusEffect(
                CardWithoutReplacements,
                nameof(ResetHP),
                $"At the start of {CharacterCard.Title}'s next turn their HP will be set to {CharacterCard.HitPoints.Value}",
                new TriggerType[] { TriggerType.Other },
                Card
            );

            effect.TurnPhaseCriteria.TurnTaker = TurnTaker;
            effect.TurnPhaseCriteria.Phase = Phase.Start;

            var e = AddStatusEffect(effect, true);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        public IEnumerator ResetHP(PhaseChangeAction pca, StatusEffect effect)
        {
            if (! HasRecordedHP()) { yield break; }

            var hp = RecordedHP();
            SetCardProperty(HPProperty, -1);

            IEnumerator e;

            // if {AlexandriaCharacter} is not incapacitated, 
            if (CharacterCard.IsIncapacitatedOrOutOfGame)
            {
                e = GameController.SendMessageAction(
                    $"{CharacterCard.Title} is incapacitated or out of the game",
                    Priority.High,
                    cardSource: GetCardSource(),
                    showCardSource: true
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
            else
            {
                // set {AlexandriaCharacter}'s HP to the value you noted down
                e = GameController.SetHP(CharacterCard, hp, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }
            }

            e = GameController.ExpireStatusEffect(effect, GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        private bool HasRecordedHP()
        {
            var recorded = GetCardPropertyJournalEntryInteger(HPProperty);
            return recorded != null && recorded.Value >= 0;
        }

        private int RecordedHP()
        {
            var recorded = GetCardPropertyJournalEntryInteger(HPProperty);
            return recorded.Value;
        }
    }
}
