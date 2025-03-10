﻿using Handelabra;
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
    public class TidalWaveKyushuCardController : KyushuOneShotCardController
    {
        public TidalWaveKyushuCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            base.AddTriggers();
        }

        public override IEnumerator Play()
        {
			// "When this card enters play, deal each hero target 2 melee damage. Each player whose hero was dealt damage this way discards 1 card."
			List<DealDamageAction> damageResults = new List<DealDamageAction>();
			IEnumerator damageCoroutine = DealDamage(base.Card, (Card c) => c.Is(this).Hero().Target(), 2, DamageType.Melee, storedResults: damageResults);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(damageCoroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(damageCoroutine);
			}
			if (damageResults.Count() > 0)
			{
				List<HeroTurnTaker> heroDiscards = new List<HeroTurnTaker>();
				IEnumerable<TurnTaker> damagedHeroes = from dd in damageResults where dd.FinalAmount > 0 && dd.Target.Is(this).Hero().Target().Character() && dd.DidDealDamage select dd.Target.Owner;
				IEnumerator discardCoroutine = base.GameController.SelectTurnTakersAndDoAction(DecisionMaker, new LinqTurnTakerCriteria((TurnTaker tt) => tt.Is(this).Hero() && damagedHeroes.Contains(tt) && !heroDiscards.Contains(tt.ToHero())), SelectionType.DiscardCard, (TurnTaker tt) => base.GameController.SelectAndDiscardCard(FindHeroTurnTakerController(tt.ToHero()), selectionType: SelectionType.DiscardCard, cardSource: GetCardSource()), allowAutoDecide: true, cardSource: GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(discardCoroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(discardCoroutine);
				}
			}
			yield break;
        }
    }
}
