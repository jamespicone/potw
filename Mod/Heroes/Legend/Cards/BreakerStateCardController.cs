using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Legend
{
    public class BreakerStateCardController : CardController
    {
        public BreakerStateCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            //"You may use an additional power each turn",
            AddAdditionalPhaseActionTrigger(tt => tt == TurnTaker, Phase.UsePower, 1);

            //"Reduce damage dealt by Legend by 1",
            AddReduceDamageTrigger(dda => dda.DamageSource.Card == CharacterCard, dda => 1);

            //"Reduce damage dealt to Legend by 1",
            AddReduceDamageTrigger(c => c == CharacterCard, 1);

            //"At the start of your turn you may destroy this card. If you do, draw a card"
            AddStartOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => DestroyToDraw(pca),
                new TriggerType[] { TriggerType.DestroySelf, TriggerType.DrawCard }
            );
        }

        private IEnumerator DestroyToDraw(PhaseChangeAction pca)
        {
            var results = new List<DestroyCardAction>();
            var e = GameController.DestroyCard(
                HeroTurnTakerController,
                Card,
                optional: true,
                storedResults: results,
                cardSource: GetCardSource(),
                responsibleCard: Card
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            if (GetNumberOfCardsDestroyed(results) > 0)
            {
                e = DrawCard();
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
