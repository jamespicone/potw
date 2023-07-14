using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Labyrinth
{
    public class VantageCardController : CardController
    {
        public VantageCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            // At the start of your turn remove any tokens on this card.",
            AddStartOfTurnTrigger(tt => tt == TurnTaker, pca => ClearTokens(), TriggerType.ModifyTokens);

            // "At the start of each other hero's turn they may put a token on this card. If they do, they may use a power."
            AddStartOfTurnTrigger(
                tt => tt.Is().Hero().AccordingTo(this) && tt != TurnTaker,
                pca => MaybeExtraPower(pca),
                new TriggerType[] { TriggerType.UsePower, TriggerType.AddTokensToPool },
                additionalCriteria: pca => CanUsePower()
            );

            AddBeforeRemovedFromPlayAction(mca => ClearTokens());
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // Destroy an environment card.
            var storedDestroys = new List<DestroyCardAction>();
            var e = GameController.SelectAndDestroyCards(
                HeroTurnTakerController,
                new LinqCardCriteria(c => c.Is().Environment().Card(), "environment"),
                numberOfCards: 1,
                requiredDecisions: 1,
                storedResultsAction: storedDestroys,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            // If you do...
            if (GetNumberOfCardsDestroyed(storedDestroys) <= 0)
                yield break;

            // ...another hero may play a card.
            e = GameController.SelectHeroToPlayCard(
                HeroTurnTakerController,
                additionalCriteria: new LinqTurnTakerCriteria(tt => tt != TurnTaker, "hero"),
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }

        private IEnumerator ClearTokens()
        {
            var pool = Card.FindTokenPool("VantagePool");
            if (pool == null) yield break;

            pool.SetToInitialValue();
            yield break;
        }

        private bool CanUsePower()
        {
            var pool = Card.FindTokenPool("VantagePool");
            if (pool == null) { return true; }

            return pool.CurrentValue == pool.InitialValue;
        }

        private IEnumerator MaybeExtraPower(PhaseChangeAction pca)
        {
            var controller = FindTurnTakerController(pca.ToPhase.TurnTaker) as HeroTurnTakerController;
            if (controller == null) yield break;

            var pool = Card.FindTokenPool("VantagePool");
            if (pool == null) yield break;

            var storedResults = new List<YesNoCardDecision>();
            var e = GameController.MakeYesNoCardDecision(
                controller,
                SelectionType.UsePower,
                Card,
                pca,
                storedResults,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            if (! DidPlayerAnswerYes(storedResults)) { yield break; }

            e = GameController.AddTokensToPool(pool, 1, GetCardSource());
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            e = GameController.SelectAndUsePower(controller, cardSource: GetCardSource());
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }
    }
}
