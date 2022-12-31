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
    public class SkitterCharacterCardController : HeroCharacterCardController
    {
        public SkitterCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator UsePower(int index = 0)
        {
            // Either place a Bug token on {SkitterCharacter} or play a Strategy.
            var pool = Card.FindBugPool();

            var storedPlay = new List<PlayCardAction>();
            var e = SelectAndPlayCardFromHand(HeroTurnTakerController, storedResults: storedPlay, cardCriteria: new LinqCardCriteria(c => c.Is().WithKeyword("strategy").AccordingTo(this)));
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            if (! DidPlayCards(storedPlay))
            {
                // adding a token instead
                e = this.AddBugTokenToSkitter(1);
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }

            if (pool != null)
            {
                // You may move a Bug token from one of your cards to {SkitterCharacter} or one of your Strategy cards.
                e = this.MoveBugTokens(moveArbitraryAmount: false, isOptional: true);
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
                    // "The environment deals 1 toxic damage to a target.",
                    var env = FindEnvironment();
                    e = GameController.SelectTargetsAndDealDamage(
                        HeroTurnTakerController,
                        new DamageSource(GameController, env.TurnTaker),
                        1,
                        DamageType.Toxic,
                        numberOfTargets: 1,
                        optional: false,
                        requiredTargets: 1,
                        cardSource: GetCardSource()
                    );
                    break;
                }
                case 1:
                {
                    // "One player draws a card."
                    e = GameController.SelectHeroToDrawCard(HeroTurnTakerController, optionalDrawCard: false, cardSource: GetCardSource());
                    break;
                }
                case 2:
                {
                    // "Destroy a non-character target. Remove Skitter from the game."
                    e = GameController.SelectAndDestroyCard(
                        HeroTurnTakerController,
                        new LinqCardCriteria(c => c.IsTarget && !c.IsCharacter, "non-character target", useCardsSuffix: false),
                        optional: false,
                        responsibleCard: Card,
                        cardSource: GetCardSource()
                    );
                    if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                    else { GameController.ExhaustCoroutine(e); }

                    e = GameController.MoveCard(
                        HeroTurnTakerController,
                        CharacterCard,
                        TurnTaker.OutOfGame,
                        cardSource: GetCardSource()
                    );
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
