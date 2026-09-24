using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Skitter
{
    public class SkitterTaylorHebertCharacterCardController : HeroCharacterCardController
    {
        public SkitterTaylorHebertCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddSideTriggers()
        {
            if (!Card.IsFlipped)
            {
                // "Whenever one of your Bug cards enters play, draw a card."
                AddSideTrigger(AddTrigger<CardEntersPlayAction>(
                    cep => cep.CardEnteringPlay.Owner == TurnTaker && cep.CardEnteringPlay.Is().WithKeyword("bug").AccordingTo(this),
                    cep => DrawCards(HeroTurnTakerController, 1),
                    TriggerType.DrawCard,
                    TriggerTiming.After
                ));
            }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // "Draw a card."
            var e = DrawCards(HeroTurnTakerController, 1);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            // No hand when the Celestial Tribunal's Representative of Earth owns us.
            if (HeroTurnTakerController == null) { yield break; }

            // "You may discard any number of cards."
            var discards = new List<DiscardCardAction>();
            e = SelectAndDiscardCards(DecisionMaker, null, optional: false, requiredDecisions: 0, storedResults: discards);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            // "Place a Bug token on {SkitterCharacter} for each card discarded this way."
            var discarded = GetNumberOfCardsDiscarded(discards);
            if (discarded > 0)
            {
                e = this.AddBugTokenToSkitter(discarded);
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator e;

            switch (index)
            {
                case 0:
                {
                    // "The environment deals each target 1 toxic damage."
                    e = GameController.SelectTargetsAndDealDamage(
                        DecisionMaker,
                        new DamageSource(GameController, FindEnvironment().TurnTaker),
                        1,
                        DamageType.Toxic,
                        numberOfTargets: null,
                        optional: false,
                        requiredTargets: null,
                        allowAutoDecide: true,
                        cardSource: GetCardSource()
                    );
                    break;
                }
                case 1:
                {
                    // "Destroy an Ongoing card."
                    e = GameController.SelectAndDestroyCard(
                        DecisionMaker,
                        new LinqCardCriteria(c => IsOngoing(c), "ongoing"),
                        optional: false,
                        cardSource: GetCardSource()
                    );
                    break;
                }
                case 2:
                {
                    // "One player may discard a card. If they do, they may draw 2 cards."
                    var discards = new List<DiscardCardAction>();
                    e = GameController.SelectHeroToDiscardCard(
                        DecisionMaker,
                        optionalSelectHero: false,
                        optionalDiscardCard: true,
                        storedResultsDiscard: discards,
                        cardSource: GetCardSource()
                    );
                    if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                    else { GameController.ExhaustCoroutine(e); }

                    if (!DidDiscardCards(discards)) { yield break; }

                    e = DrawCards(discards.First().HeroTurnTakerController, 2, optional: true);
                    break;
                }
                default:
                    yield break;
            }

            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }
    }
}
