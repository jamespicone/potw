using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Dauntless
{
    public class CarryTheChargeCardController : CardController
    {
        public CarryTheChargeCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            // Whenever {DauntlessCharacter} is dealt damage you may draw a card. If it was energy or lightning damage you may play a card as well
            AddTrigger<DealDamageAction>(
                dda => dda.Target == CharacterCard,
                dda => DrawAndMaybePlay(dda),
                new TriggerType[] { TriggerType.DrawCard, TriggerType.PlayCard },
                TriggerTiming.After
            );
        }

        private IEnumerator DrawAndMaybePlay(DealDamageAction dda)
        {
            var e = DrawCard(HeroTurnTaker, optional: true);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            if (dda.DamageType == DamageType.Energy || dda.DamageType == DamageType.Lightning)
            {
                e = SelectAndPlayCardFromHand(HeroTurnTakerController);
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
}
