using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Legend
{
    public class BounceBackCardController : CardController
    {
        public BounceBackCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override bool AllowFastCoroutinesDuringPretend { get => ! HasChargeTokens(); }

        public override void AddTriggers()
        {
            //"Whenever Legend takes damage place a Charge token on this card",
            AddTrigger<DealDamageAction>(
                dda => dda.Target == CharacterCard && dda.DidDealDamage,
                dda => AddChargeToken(),
                TriggerType.AddTokensToPool,
                TriggerTiming.After
            );

            //"Whenever Legend deals damage you may remove a Charge token from this card to increase the damage by 1"
            AddTrigger<DealDamageAction>(
                dda => dda.DamageSource.Card == CharacterCard && HasChargeTokens(),
                dda => UseChargeToken(dda),
                TriggerType.IncreaseDamage,
                TriggerTiming.Before
            );
        }

        private IEnumerator AddChargeToken()
        {
            var pool = Card.FindTokenPool("ChargePool");
            if (pool == null) { yield break; }

            var e = GameController.AddTokensToPool(pool, 1, GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        private bool HasChargeTokens()
        {
            var pool = Card.FindTokenPool("ChargePool");
            if (pool == null) { return false; }

            return pool.CurrentValue > 0;
        }

        private IEnumerator UseChargeToken(DealDamageAction dda)
        {
            var pool = Card.FindTokenPool("ChargePool");
            if (pool == null) { yield break; }

            if (! HasChargeTokens()) { yield break; }

            if (GameController.PretendMode || increaseDamage == null)
            {
                var increaseResult = new List<YesNoCardDecision>();
                var e = GameController.MakeYesNoCardDecision(
                    HeroTurnTakerController,
                    SelectionType.RemoveTokens,
                    Card,
                    dda,
                    increaseResult,
                    new Card[] { dda.Target },
                    GetCardSource()
                );
                
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }

                increaseDamage = DidPlayerAnswerYes(increaseResult);
            }

            if (increaseDamage.GetValueOrDefault(false))
            {
                var e = GameController.RemoveTokensFromPool(pool, 1, gameAction: dda, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }

                e = GameController.IncreaseDamage(dda, 1, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }
            }

            if (! GameController.PretendMode)
            {
                increaseDamage = null;
            }
        }

        private bool? increaseDamage;
    }
}
