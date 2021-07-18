using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Dauntless
{
    public class MatterToEnergyCardController : CardController
    {
        public MatterToEnergyCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowListOfCards(TargetCriteria());
            SpecialStringMaker.ShowCardThisCardIsNextTo(Card);
        }

        public override void AddTriggers()
        {
            AddIfTheTargetThatThisCardIsNextToLeavesPlayDestroyThisCardTrigger();
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            // Play this card next to {DauntlessCharacter} or a Relic in this play area without a Matter to Energy
            var e = SelectCardThisCardWillMoveNextTo(
                TargetCriteria(),
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

        private LinqCardCriteria TargetCriteria()
        {
            return new LinqCardCriteria(
                c =>
                    (
                        c == CharacterCard ||
                        (c.DoKeywordsContain("relic") && c.Location == TurnTaker.PlayArea)
                    ) && !c.HasMatterToEnergy(),
                "Dauntless or a Relic without a Matter to Energy"
            );
        }
    }
}
