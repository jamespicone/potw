using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Jp.ParahumansOfTheWormverse.Leviathan
{

    // Whenever a player would draw a card, Leviathan deals that player's character card 1 cold damage. This card is indestructible

    public class TorrentialDownpourCardController : CardController
    {
        public TorrentialDownpourCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            //This card is indestructible
            base.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override void AddTriggers()
        {
            //Whenever a player would draw a card...  
            AddTrigger<DrawCardAction>(
                dca => dca.HeroTurnTaker != null,
                dca => RespondToCardDraw(dca),
                TriggerType.DealDamage,
                TriggerTiming.Before

            /*
             Trigger is triggering off drawing card, but IS dealing damage - unclear whether to use TriggerType.DealDamage or TriggerType.DrawCard
             I THINK DealDamage, since "DrawCard" insinuates that in that setting the trigger is 'dealing damage', but clarification definitely would be good
             Relevant doc section: https://docs.google.com/document/d/e/2PACX-1vRvUNq-KAWwLdvQmhjpFp-dC6s7ZJqogQJFIFfCZrhJ6_kuS9yi5KG-OmEU3g2NqsB0zkMS0KPtTC5V/pub#h.vzv3ofjqd8e2
             */
            );
        }

        public IEnumerator RespondToCardDraw(DrawCardAction dca)
        {
            //...Leviathan deals that player's character card 1 cold damage
            var e = DealDamage(CharacterCard, dca.HeroTurnTaker.CharacterCard, 1, DamageType.Cold, true);
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
