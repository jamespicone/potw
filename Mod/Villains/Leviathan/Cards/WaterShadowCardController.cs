using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Jp.ParahumansOfTheWormverse.Leviathan
{
    public class WaterShadowCardController : CardController
    {
        public WaterShadowCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            // This card is indestructible
            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            return card == Card;
        }

        public override void AddTriggers()
        {
            // Whenever Leviathan deals melee damage to a target...  
            AddTrigger<DealDamageAction>(
                dda => dda.DamageType == DamageType.Melee && dda.DamageSource.Card == CharacterCard && dda.IsSuccessful && dda.Amount > 0,
                dda => RespondToMeleeDamage(dda),
                TriggerType.DealDamage,
                TriggerTiming.After
            );
        }

        public IEnumerator RespondToMeleeDamage(DealDamageAction dda)
        {
            // ...this card deals that target 2 irreducible melee damage
            var e = DealDamage(Card, dda.Target, 2, DamageType.Melee, isIrreducible: true, cardSource: GetCardSource());
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
