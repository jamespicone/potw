using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Echidna
{
    // TODO: Unit tests
    public class EchidnaCharacterCardController : VillainCharacterCardController
    {
        public EchidnaCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowIfElseSpecialString(
                () => NoHeroWasDamagedByVillain(),
                () => "No hero has been dealt damage by a villain card this round",
                () => "A hero has been dealt damage by a villain card this round"
            );
        }

        public override void AddSideTriggers()
        {
            AddDefeatedIfDestroyedTriggers();

            if (Card.IsFlipped)
            {
                // At the start of the villain turn search the villain deck for an 'Engulfed' card, put it into play, and shuffle the villain deck. Then flip {EchidnaCharacter}.
                AddSideTrigger(AddStartOfTurnTrigger(
                    tt => tt == TurnTaker,
                    pca => EngulfAndFlip(),
                    new TriggerType[] {
                        TriggerType.PutIntoPlay,
                        TriggerType.FlipCard
                    }
                ));
            }
            else
            {
                // At the start of the villain turn {EchidnaCharacter} regains {H} HP.
                AddSideTrigger(AddStartOfTurnTrigger(
                    tt => tt == TurnTaker,
                    pca => GameController.GainHP(Card, H, cardSource: GetCardSource()),
                    TriggerType.GainHP
                ));

                // At the end of the villain turn {EchidnaCharacter} deals {H - 1} melee damage to the 2 hero targets with the lowest HP.
                AddSideTrigger(AddDealDamageAtEndOfTurnTrigger(
                    TurnTaker,
                    Card,
                    c => c.Is().Hero().Target(),
                    TargetType.LowestHP,
                    H - 1,
                    DamageType.Melee,
                    numberOfTargets: 2
                ));

                // At the end of the environment turn if no hero was dealt damage by a villain card this round flip {EchidnaCharacter}.
                AddSideTrigger(AddEndOfTurnTrigger(
                    tt => tt == FindEnvironment().TurnTaker,
                    pca => GameController.FlipCard(this, cardSource: GetCardSource()),
                    TriggerType.FlipCard,
                    pca => NoHeroWasDamagedByVillain()
                ));

                // "advanced": "Reduce damage dealt to {EchidnaCharacter} by 1."
                if (IsGameAdvanced)
                {
                    AddSideTrigger(AddReduceDamageTrigger(c => c == Card, 1));
                }
            }
        }

        private bool NoHeroWasDamagedByVillain()
        {
            return Journal.DealDamageEntriesThisRound()
                .Where(dd => dd.TargetCard.Is().Hero().Character())
                .Where(dd => dd.SourceCard.Is().Villain().AccordingTo(this))
                .Count() <= 0;
        }

        private IEnumerator EngulfAndFlip()
        {
            var e = PlayCardFromLocation(TurnTaker.Deck, "Engulfed");
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            e = GameController.FlipCard(this, cardSource: GetCardSource());
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            var e = base.AfterFlipCardImmediateResponse();
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            if (! Card.IsFlipped)
            {
                yield break;
            }

            // When {EchidnaCharacter} flips to this side destroy {H} noncharacter hero cards.
            e = GameController.SelectAndDestroyCards(
                DecisionMaker,
                new LinqCardCriteria(c => c.Is().Hero().Noncharacter(), "hero cards"),
                numberOfCards: Game.H,
                responsibleCard: Card,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            if (IsGameAdvanced)
            {
                // When {EchidnaCharacter} flips to this side search the villain deck for an 'Engulfed' card, put it into play, and shuffle the villain deck.
                e = PlayCardFromLocation(TurnTaker.Deck, "Engulfed");
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }
        }
    }
}
