using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Dauntless
{
    public class PlasmaCoreCardController : CardController
    {
        public PlasmaCoreCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            // Play this card next to {DauntlessCharacter} or a Relic in this play area without a Plasma Core
            var e = SelectCardThisCardWillMoveNextTo(
                new LinqCardCriteria(
                    c =>
                        (
                            c == CharacterCard ||
                            (c.DoKeywordsContain("relic") && c.Location == TurnTaker.PlayArea)
                        ) && ! c.HasPlasmaCore(),
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
    }
}
