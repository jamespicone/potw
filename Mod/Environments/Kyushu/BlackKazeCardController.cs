using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Kyushu
{
    public class BlackKazeCardController : CardController
    {
        public BlackKazeCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowLowestHP(1, null, new LinqCardCriteria((Card c) => c.IsTarget && c.IsCharacter, "character card target", false)).Condition = () => GetAttackedCard() == null;
            SpecialStringMaker.ShowLowestHP(1, null, new LinqCardCriteria((Card c) => c.IsTarget && c.IsCharacter && c != GetAttackedCard(), "character card target other than the target Black Kaze is attacking", false)).Condition = () => GetAttackedCard() != null;
            SpecialStringMaker.ShowIfElseSpecialString(() => HasBeenSetToTrueThisRound(OncePerRound), () => base.Card.Title + " has redirected damage this round.", () => base.Card.Title + " has not redirected damage this round.").Condition = () => base.Card.IsInPlayAndHasGameText;
        }

        protected const string OncePerRound = "RedirectOncePerRound";
        private ITrigger RedirectTrigger;
        private const string Victim = "Victim";

        public override void AddTriggers()
        {
            // "At the end of the environment turn, move this card next to the character card target other than the target this card is already next to with the lowest HP. Then, this card deals the target it is next to 3 irreducible melee damage."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, MoveAndAttackResponse, new TriggerType[] { TriggerType.MoveCard, TriggerType.DealDamage });
            // "The first time this card would be dealt damage each round, redirect that damage to the target this card is next to."
            this.RedirectTrigger = base.AddTrigger<DealDamageAction>((DealDamageAction dda) => !HasBeenSetToTrueThisRound(OncePerRound) && dda.Target == base.Card && dda.Amount > 0, this.RedirectResponse, TriggerType.RedirectDamage, TriggerTiming.Before);
            // implied: if Kaze's victim leaves play, she stays in play in their play area
            AddIfTheCardThatThisCardIsNextToLeavesPlayMoveItToTheirPlayAreaTrigger(false);
            AddAfterLeavesPlayAction(ResetVictim);
            base.AddTriggers();
        }

        public IEnumerator MoveAndAttackResponse(PhaseChangeAction pca)
        {
            // "... move this card next to the character card target other than the target this card is already next to with the lowest HP."
            List<Card> lowestResults = new List<Card>();
            IEnumerator findCoroutine = base.GameController.FindTargetWithLowestHitPoints(1, (Card c) => c.IsCharacter && c.IsTarget && c.IsInPlayAndHasGameText && c != GetAttackedCard(), lowestResults, evenIfCannotDealDamage: true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(findCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(findCoroutine);
            }
            Card lowest = lowestResults.FirstOrDefault();
            if (lowest != null)
            {
                IEnumerator moveCoroutine;
                if (!base.Card.GetAllNextToCards(true).Contains(lowest))
                {
                    SetVictim(null);
                    moveCoroutine = base.GameController.MoveCard(base.TurnTakerController, base.Card, lowest.NextToLocation, responsibleTurnTaker: base.TurnTaker, cardSource: GetCardSource());
                }
                else
                {
                    SetVictim(lowest);
                    IEnumerator messageCoroutine = base.GameController.SendMessageAction(base.Card.Title + " attacks " + lowest.Title + " from the environment play area.", Priority.Medium, GetCardSource(), showCardSource: true);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(messageCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(messageCoroutine);
                    }
                    moveCoroutine = base.GameController.MoveCard(base.TurnTakerController, base.Card, FindEnvironment().TurnTaker.PlayArea, responsibleTurnTaker: base.TurnTaker, cardSource: GetCardSource());
                }
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(moveCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(moveCoroutine);
                }
            }
            // "Then, this card deals the target it is next to 3 irreducible melee damage."
            Card target = GetAttackedCard();
            if (target.IsTarget && target.IsInPlayAndHasGameText)
            {
                IEnumerator damageCoroutine = DealDamage(base.Card, target, 3, DamageType.Melee, isIrreducible: true, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(damageCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(damageCoroutine);
                }
            }
            yield break;
        }

        public IEnumerator RedirectResponse(DealDamageAction dda)
        {
            // "... redirect that damage to the target this card is next to."
            base.SetCardPropertyToTrueIfRealAction(OncePerRound);
            Card victim = GetAttackedCard();
            if (victim != null && victim.IsInPlayAndHasGameText && victim.IsTarget)
            {
                IEnumerator redirectCoroutine = base.GameController.RedirectDamage(dda, victim, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(redirectCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(redirectCoroutine);
                }
            }
            else
            {
                IEnumerator messageCoroutine = base.GameController.SendMessageAction(base.Card.Title + " has no valid target to redirect damage to!", Priority.Medium, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(messageCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(messageCoroutine);
                }
            }
            yield break;
        }

        private void SetVictim(Card card)
        {
            AddCardPropertyJournalEntry(nameof(Victim), card);
        }

        private Card GetVictim()
        {
            return GetCardPropertyJournalEntryCard(nameof(Victim));
        }

        private Card GetAttackedCard()
        {
            Card victim = GetVictim();
            if (victim != null)
            {
                return victim;
            }
            else
            {
                return GetCardThisCardIsNextTo();
            }
        }

        private IEnumerator ResetVictim()
        {
            SetVictim(null);
            yield return null;
        }
    }
}
