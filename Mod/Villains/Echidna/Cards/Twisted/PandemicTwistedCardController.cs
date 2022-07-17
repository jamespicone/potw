using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Jp.SOTMUtilities;
using System.Collections;

using System;

namespace Jp.ParahumansOfTheWormverse.Echidna
{
    public class PandemicTwistedCardController : CardController
    {
        public PandemicTwistedCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowHasBeenUsedThisTurn(
                "PandemicPreventedDestruction",
                "{0} has saved a villain target this round",
                "{0} has not saved a villain target this round"
            );
        }

        public override void AddTriggers()
        {
            // At the end of the villain turn all villain targets regain 2 HP.
            AddEndOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => GameController.GainHP(
                    DecisionMaker,
                    c => c.Is().Villain().Target().AccordingTo(this),
                    2,
                    cardSource: GetCardSource()
                ),
                TriggerType.GainHP
            );

            // The first time each round a villain target would be reduced to 0 or fewer HP
            // restore that target's HP to 1.
            AddTrigger<DestroyCardAction>(
                dca => !HasBeenSetToTrueThisRound("PandemicPreventedDestruction") &&
                    dca.CardToDestroy.Is().Villain().Target().AccordingTo(this) &&
                    dca.CardToDestroy.Card.HitPoints <= 0,
                dca => PreventAndSetHP(dca),
                new TriggerType[] { TriggerType.GainHP, TriggerType.CancelAction },
                TriggerTiming.Before
            );

            AddTrigger<DealDamageAction>(
                dda => !HasBeenSetToTrueThisRound("PandemicPreventedDestruction") &&
                    dda.Target.Is().Villain().Target().AccordingTo(this) &&
                    dda.TargetHitPointsAfterBeingDealtDamage <= 0,
                dda => PreventAndSetHP(dda),
                new TriggerType[] { TriggerType.GainHP, TriggerType.CancelAction },
                TriggerTiming.After
            );
        }

        private IEnumerator PreventAndSetHP(GameAction dca)
        {
            Card target = null;
            if (dca is DealDamageAction)
            {
                target = ((DealDamageAction)dca).Target;
            }

            if (dca is DestroyCardAction)
            {
                target = ((DestroyCardAction)dca).CardToDestroy.Card;
            }

            if (target == null)
            {
                throw new ArgumentException($"dca argument should be dealdamageaction or destroycardaction, was {dca}");
            }

            SetCardPropertyToTrueIfRealAction("PandemicPreventedDestruction", gameAction: dca);
            var e = CancelAction(dca, isPreventEffect: true);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            var hp = Math.Min(1, target.MaximumHitPoints ?? 0);
            e = GameController.SetHP(target, hp, GetCardSource());
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }
    }
}
