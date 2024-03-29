using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Dauntless
{
    public class CrystallizationCardController : CardController
    {
        public CrystallizationCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            AddThisCardControllerToList(CardControllerListType.ChangesVisibility);
            SpecialStringMaker.ShowCardThisCardIsNextTo(Card);
        }

        public override void AddTriggers()
        {
            AddTrigger<DestroyCardAction>(dca => dca.CardToDestroy.Card == Card, dca => ReturnToHand(dca), TriggerType.CancelAction, TriggerTiming.Before);
            AddIfTheTargetThatThisCardIsNextToLeavesPlayDestroyThisCardTrigger();
        }

        public override IEnumerator Play()
        {
            // "When this card enters play {DauntlessCharacter} may deal 1 energy damage to a target",
            var e = GameController.SelectTargetsAndDealDamage(
                HeroTurnTakerController,
                new DamageSource(GameController, CharacterCard),
                1,
                DamageType.Energy,
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

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            // Play this card next to {DauntlessCharacter} or a Relic in this play area",
            var e = SelectCardThisCardWillMoveNextTo(
                new LinqCardCriteria(
                    c =>
                        c == CharacterCard || 
                        (c.DoKeywordsContain("relic") && c.Location == TurnTaker.PlayArea),
                    "Dauntless or a Relic"
                ),
                storedResults,
                isPutIntoPlay,
                decisionSources
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

        public override bool? AskIfCardIsVisibleToCardSource(Card card, CardSource cardSource)
        {
            // This card cannot be affected by villain or environment cards
            if (card != Card) { return null; }
            if (cardSource.Card == null) { return null; }

            if (cardSource.Card.Is(this).Villain() || cardSource.Card.Is().Environment()) { return false; }

            return null;
        }

        private IEnumerator ReturnToHand(DestroyCardAction dca)
        {
            var e = CancelAction(dca);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            e = GameController.MoveCard(
                TurnTakerController,
                Card,
                HeroTurnTaker.Hand,
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
