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
                // "Whenever you play a Bug card, draw a card."
                AddSideTrigger(AddTrigger<PlayCardAction>(
                    pca => pca.WasCardPlayed && !pca.IsPutIntoPlay && pca.CardToPlay.Owner == TurnTaker &&
                        pca.CardToPlay.Is().WithKeyword("bug").AccordingTo(this),
                    pca => DrawCards(HeroTurnTakerController, 1),
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
                    var source = new DamageSource(GameController, FindEnvironment().TurnTaker);
                    foreach (var target in GameController.FindTargetsInPlay().ToList())
                    {
                        if (!target.IsInPlayAndHasGameText || !target.IsTarget) { continue; }

                        e = GameController.DealDamageToTarget(source, target, 1, DamageType.Toxic, cardSource: GetCardSource());
                        if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                        else { GameController.ExhaustCoroutine(e); }
                    }
                    yield break;
                }
                case 1:
                {
                    // "Destroy an Ongoing card."
                    e = GameController.SelectAndDestroyCard(
                        DecisionMaker,
                        new LinqCardCriteria(c => c.IsOngoing, "ongoing"),
                        optional: false,
                        cardSource: GetCardSource()
                    );
                    break;
                }
                case 2:
                {
                    // "One player may discard a card. If they do, they may draw 2 cards."
                    var selected = new List<SelectTurnTakerDecision>();
                    e = GameController.SelectHeroTurnTaker(
                        DecisionMaker,
                        SelectionType.DiscardCard,
                        optional: false,
                        allowAutoDecide: false,
                        selected,
                        heroCriteria: new LinqTurnTakerCriteria(tt => tt.ToHero().HasCardsInHand),
                        cardSource: GetCardSource()
                    );
                    if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                    else { GameController.ExhaustCoroutine(e); }

                    var player = GetSelectedTurnTaker(selected);
                    if (player == null) { yield break; }
                    var playerController = FindHeroTurnTakerController(player.ToHero());

                    var discards = new List<DiscardCardAction>();
                    e = SelectAndDiscardCards(playerController, 1, optional: true, storedResults: discards, responsibleTurnTaker: player);
                    if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                    else { GameController.ExhaustCoroutine(e); }

                    if (!DidDiscardCards(discards)) { yield break; }

                    e = DrawCards(playerController, 2, optional: true);
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
