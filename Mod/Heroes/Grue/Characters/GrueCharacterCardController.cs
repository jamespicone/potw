using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.Grue
{
    public class GrueCharacterCardController : HeroCharacterCardController
    {
        public GrueCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            AddAsPowerContributor();
        }

        public override IEnumerable<Power> AskIfContributesPowersToCardController(CardController cardController)
        {
            if (cardController != this) { return null; }

            // if (we haven't second triggered) { return null; }

            return new Power[] {
                new Power(
                    cardController.HeroTurnTakerController,
                    cardController,
                    "Select a player. That player may use a power now",
                    SecondTriggerPower(),
                    0,
                    null,
                    GetCardSource()
                )
            };
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator e;
            switch(index)
            {
                case 0:
                case 1:
                case 2:
                default: yield break;
            }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            if (index == 0)
            {
                return PutDarknessesIntoPlay();
            }
            else
            {
                return SecondTriggerPower();
            }
        }

        private IEnumerator PutDarknessesIntoPlay()
        {
            // Put a Darkness card into play next to {GrueCharacter}. Put a Darkness card into play next to another target.
            var e = this.PutDarknessIntoPlay(CharacterCard);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            var selectedTarget = new List<SelectCardDecision>();
            e = GameController.SelectCardAndStoreResults(
                HeroTurnTakerController,
                SelectionType.MoveCardNextToCard,
                new LinqCardCriteria(c => c.IsTarget && c.IsInPlay && c != CharacterCard, "target"),
                storedResults: selectedTarget,
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

            var target = GetSelectedCard(selectedTarget);
            if (target == null) {  yield break; }

            e = this.PutDarknessIntoPlay(target);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        private IEnumerator SecondTriggerPower()
        {
            // Use this power only if you can use {Trigger} powers{BR}Select a player. That player may use a power now
            return GameController.SelectHeroToUsePower(HeroTurnTakerController, cardSource: GetCardSource());
        }
    }
}
