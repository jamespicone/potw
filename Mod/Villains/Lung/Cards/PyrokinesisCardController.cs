using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Jp.ParahumansOfTheWormverse.Lung
{
    public class PyrokinesisCardController : CardController
    {
        public PyrokinesisCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            // Whenever {Lung} deals melee damage to a target, {Lung} deals 2 fire damage to that target
            AddTrigger<DealDamageAction>(
                dda => dda.DamageType == DamageType.Melee && dda.DamageSource.Card == CharacterCard && dda.DidDealDamage,
                dda => RespondToMeleeDamage(dda),
                TriggerType.DealDamage,
                TriggerTiming.After
            );
        }

        public IEnumerator RespondToMeleeDamage(DealDamageAction dda)
        {
            var e = DealDamage(CharacterCard, dda.Target, 2, DamageType.Fire, cardSource: GetCardSource());
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
