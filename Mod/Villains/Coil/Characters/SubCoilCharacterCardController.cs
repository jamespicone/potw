using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Coil
{
    public abstract class SubCoilCharacterCardController : VillainCharacterCardController
    {
        public SubCoilCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
        }

        protected Card OtherCoil
        {
            get
            {
                var c1 = FindCard("CoilActingCharacter");
                var c2 = FindCard("CoilSchemingCharacter");

                if (Card == c1) { return c2; }
                else { return c1; }
            }
        }

        protected abstract void AddCoilTriggers();

        public override sealed void AddSideTriggers()
        {
            if (! Card.IsFlipped)
            {
                AddCoilTriggers();
            }

            if (IsGameAdvanced)
            {
                // "At the end of the villain turn set this card's HP to the greater of it's current HP and Scheming's HP
                // "At the end of the villain turn flip this card and set its HP to Scheming's HP"
                AddEndOfTurnTrigger(
                    tt => tt == TurnTaker,
                    pca => EqualiseCoils(),
                    TriggerType.GainHP
                );
            }
        }

        public override bool CanBeDestroyed => false;

        public override IEnumerator DestroyAttempted(DestroyCardAction destroyCard)
        {
            // "When this card would be destroyed flip it instead"
            if (Card.IsFlipped)
            {
                yield return base.DestroyAttempted(destroyCard);
            }
            else
            {
                var e1 = GameController.RemoveTarget(Card, cardSource: GetCardSource());
                var e2 = GameController.FlipCard(this, actionSource: destroyCard, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e1);
                    yield return GameController.StartCoroutine(e2);
                }
                else
                {
                    GameController.ExhaustCoroutine(e1);
                    GameController.ExhaustCoroutine(e2);
                }
            }
        }

        protected bool UseExtraText()
        {
            var otherHP = OtherCoil?.HitPoints ?? 0;
            if (! OtherCoil.IsInPlayAndHasGameText)
            {
                otherHP = 0;
            }

            return (Card.HitPoints ?? 0) >= otherHP;
        }

        private IEnumerator EqualiseCoils()
        {
            if (Card.IsFlipped)
            {
                // Revive, but only if other coil is around
                if (! OtherCoil.IsInPlayAndHasGameText) { yield break; }

                var e1 = GameController.FlipCard(this, cardSource: GetCardSource());
                var e2 = GameController.ChangeMaximumHP(Card, Card.MaximumHitPoints ?? 0, alsoSetHP: true, cardSource: GetCardSource());
                var e3 = GameController.SetHP(Card, OtherCoil.HitPoints ?? 0, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e1);
                    yield return GameController.StartCoroutine(e2);
                    yield return GameController.StartCoroutine(e3);
                }
                else
                {
                    GameController.ExhaustCoroutine(e1);
                    GameController.ExhaustCoroutine(e2);
                    GameController.ExhaustCoroutine(e3);
                }
            }
            else
            {
                if (OtherCoil.HitPoints < Card.HitPoints || ! OtherCoil.IsInPlayAndHasGameText) { yield break; }

                var e = GameController.SetHP(Card, OtherCoil.HitPoints ?? 0, GetCardSource());
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
