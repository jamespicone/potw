using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Alexandria
{
    public class HypersonicFlightCardController : CardController
    {
        public HypersonicFlightCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            // "When this card enters play {AlexandriaCharacter} may deal 2 melee damage to a target",
            var e = GameController.SelectTargetsAndDealDamage(
                HeroTurnTakerController,
                new DamageSource(GameController, CharacterCard),
                amount: 2,
                DamageType.Melee,
                numberOfTargets: 1,
                optional: false,
                requiredTargets: 0,
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

        public override void AddTriggers()
        {
            // "When {AlexandriaCharacter} deals damage to a villain target you may destroy a Device or Ongoing card"
            AddTrigger<DealDamageAction>(
                dda => dda.DidDealDamage && dda.DamageSource.Card == CharacterCard && dda.Target.IsVillainTarget,
                dda => DestroyACard(),
                TriggerType.DestroyCard,
                TriggerTiming.After
            );
        }

        private IEnumerator DestroyACard()
        {
            var e = GameController.SelectAndDestroyCard(
                HeroTurnTakerController,
                new LinqCardCriteria(c => c.IsDevice || c.DoKeywordsContain("equipment"), "device or equipment"),
                optional: true,
                responsibleCard: Card,
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
    }
}
