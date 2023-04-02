using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Kyushu
{
    public class CollapsingBuildingCardController : CardController
    {
        public CollapsingBuildingCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
			SpecialStringMaker.ShowHighestHP(2, null, new LinqCardCriteria((Card c) => c.IsTarget));
        }

        public override void AddTriggers()
        {
            // "At the end of the environment turn, deal 4 irreducible melee damage to the target with the second highest HP, then destroy this card."
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, AttackAndDestroyResponse, new TriggerType[] { TriggerType.DealDamage, TriggerType.DestroySelf });
            base.AddTriggers();
        }

        public override IEnumerator Play()
        {
			// "When this card enters play, 1 player may discard 2 cards. If they do, destroy this card."
			if (base.GameController.AllHeroes.Any((HeroTurnTaker hero) => hero.Hand.Cards.Count() >= 2))
			{
				List<SelectTurnTakerDecision> storedResults = new List<SelectTurnTakerDecision>();
				IEnumerator coroutine = base.GameController.SelectHeroTurnTaker(null, SelectionType.DiscardCard, optional: true, allowAutoDecide: false, storedResults, new LinqTurnTakerCriteria((TurnTaker tt) => tt.Is(this).Hero() && tt.ToHero().Hand.Cards.Count() >= 2, "heroes with at least 2 cards in hand"), 2, allowIncapacitatedHeroes: false, null, null, canBeCancelled: true, null, GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
				TurnTaker turnTaker = (from d in storedResults
									   where d.Completed
									   select d.SelectedTurnTaker).FirstOrDefault();
				if (turnTaker != null)
				{
					IEnumerator coroutine2 = SelectAndDiscardCards(base.GameController.FindTurnTakerController(turnTaker).ToHero(), 2);
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(coroutine2);
					}
					else
					{
						base.GameController.ExhaustCoroutine(coroutine2);
					}
					IEnumerator coroutine3 = base.GameController.DestroyCard(DecisionMaker, base.Card, responsibleCard: base.Card, cardSource: GetCardSource());
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(coroutine3);
					}
					else
					{
						base.GameController.ExhaustCoroutine(coroutine3);
					}
				}
			}
			else
			{
				IEnumerator coroutine4 = base.GameController.SendMessageAction("There are no heroes with enough cards in their hand to destroy " + base.Card.Title + ".", Priority.High, GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine4);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine4);
				}
				Log.Debug(LogName.MegaComputer, "There are no heroes with enough cards in their hand to destroy " + base.Card.Title + ".");
			}
			yield break;
        }

        public IEnumerator AttackAndDestroyResponse(PhaseChangeAction pca)
        {
			// "... deal 4 irreducible melee damage to the target with the second highest HP..."
			IEnumerator damageCoroutine = DealDamageToHighestHP(base.Card, 2, (Card c) => c.IsTarget, (Card c) => 4, DamageType.Melee, true);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(damageCoroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(damageCoroutine);
			}
			// "... then destroy this card."
			IEnumerator destroyCoroutine = base.GameController.DestroyCard(DecisionMaker, base.Card, false, responsibleCard: base.Card, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(destroyCoroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(destroyCoroutine);
			}
			yield break;
        }
    }
}
