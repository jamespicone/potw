using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Jp.ParahumansOfTheWormverse.Leviathan
{

    // Whenever Leviathan would deal melee damage to a target, this card deals that target 2 irreducible melee damage. This card is indestructible

    public class WaterShadowCardController : CardController
    {
        public WaterShadowCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            //This card is indestructible
            base.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override void AddTriggers()
        {
          //Whenever Leviathan would deal melee damage to a target...  
            AddTrigger<DealDamageAction>(
                dda => dda.DamageType == DamageType.Melee && dda.DamageSource.Card == CharacterCard,
                dda => RespondToMeleeDamage(dda),
                TriggerType.DealDamage,
                TriggerTiming.Before,
                ActionDescription.DamageTaken
            );
        }

        public IEnumerator RespondToMeleeDamage(DealDamageAction dda)
        {
            //...this card deals that target 2 irreducible melee damage
            var e = DealDamage(this.Card, dda.Target, 2, DamageType.Melee, true);
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
