using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Dauntless
{
    public class CrystallizationCardController : CardController
    {
        public CrystallizationCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
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

        public override bool AskIfCardIsIndestructible(Card card)
        {
            // "This card is indestructible"
            return card == Card;
        }
    }
}
