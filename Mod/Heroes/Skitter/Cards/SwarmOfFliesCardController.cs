using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Skitter
{
    public class SwarmOfFliesCardController : CardController
    {
        public SwarmOfFliesCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            // Reduce melee and projectile damage dealt to targets in this play area by 1.
            AddReduceDamageTrigger(
                dda => (dda.DamageType == DamageType.Melee || dda.DamageType == DamageType.Projectile) && dda.Target.IsAtLocationRecursive(Card.Location.HighestRecursiveLocation),
                dda => 1
            );

            AddAfterLeavesPlayAction(ga => LoseAToken(), TriggerType.ModifyTokens);
        }

        public override IEnumerator Play()
        {
            // When this card enters play put 2 Bug tokens on {SkitterCharacter}.
            return this.AddBugTokenToSkitter(2);
        }

        private IEnumerator LoseAToken()
        {
            AddInhibitorException(ga => ga is RemoveTokensFromPoolAction);

            var selectedCardList = new List<SelectCardDecision>();
            var e = GameController.SelectCardAndStoreResults(
                HeroTurnTakerController,
                SelectionType.RemoveTokens,
                new LinqCardCriteria(c => c.FindBugPool()?.CurrentValue > 0),
                selectedCardList,
                optional: false,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            var selectedCard = GetSelectedCard(selectedCardList);
            var pool = selectedCard?.FindBugPool();
            if (pool != null)
            {
                e = GameController.RemoveTokensFromPool(pool, 1, cardSource: GetCardSource());
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }

            RemoveInhibitorException();
        }
    }
}
