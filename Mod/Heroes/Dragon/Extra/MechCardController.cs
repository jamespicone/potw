using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Dragon
{
    public abstract class MechCardController : CardController
    {
        public MechCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
        }

        protected abstract int FocusCost();

        protected abstract IEnumerator HandleOtherAbilities(CardDefinition.ActivatableAbilityDefinition definition);

        protected virtual void AddExtraTriggers() { }

        public override void AddTriggers()
        {
            AddStartOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => CharacterCardController.LoseFocusTokens(FocusCost(), GetCardSource()),
                TriggerType.ModifyTokens
            );

            AddExtraTriggers();
        }

        public override IEnumerator ActivateAbilityEx(CardDefinition.ActivatableAbilityDefinition definition)
        {
            if (definition.Name == "focus" && definition.Number == 0)
            {
                // Return this card to your hand.
                var e = GameController.MoveCard(
                    TurnTakerController,
                    Card,
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
            else
            {
                yield return HandleOtherAbilities(definition);
            }
        }
    }
}
