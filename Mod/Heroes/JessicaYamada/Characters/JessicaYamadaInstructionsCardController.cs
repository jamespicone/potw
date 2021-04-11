using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Jp.ParahumansOfTheWormverse.JessicaYamada
{
    public class JessicaYamadaInstructionsCardController : HeroCharacterCardController
    {
        public JessicaYamadaInstructionsCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddSideTriggers()
        {
            Debug.Log("JY Instructions adding side triggers");

            if (! Card.IsFlipped)
            {
                AddSideTrigger(AddTrigger<GameAction>(
                    (ga) => ((ga is FlipCardAction || ga is BulkRemoveTargetsAction || ga is MoveCardAction) && !Card.IsFlipped),
                    (ga) => IncapacitateIfShouldBeIncapped(),
                    TriggerType.FlipCard,
                    TriggerTiming.After
                ));
            }
            else
            {
                AddSideTriggers(AddTargetEntersPlayTrigger(
                    (c) => Card.IsFlipped && CharacterCards.Contains(c),
                    (c) => GameController.FlipCard(FindCardController(Card), cardSource: GetCardSource()),
                    TriggerType.Hidden,
                    TriggerTiming.After,
                    outOfPlayTrigger: true
                ));
            }
        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            Debug.Log("JY Instructions AfterFlipCard blah blah");
            RemoveAllTriggers();
            AddSideTriggers();
            yield return null;
        }

        public override void AddStartOfGameTriggers()
        {
            if (Card.IsIncapacitatedOrOutOfGame) { return; }

            var hero = FindCard("JessicaYamadaCharacter");
            TurnTaker.MoveCard(hero, TurnTaker.PlayArea);
        }

        public IEnumerator IncapacitateIfShouldBeIncapped()
        {
            if (TurnTaker.IsIncapacitatedOrOutOfGame) { yield break; }
            if (GameController.FindTurnTakersWhere(tt => tt.IsHero && tt != TurnTaker && !tt.IsIncapacitatedOrOutOfGame).Count() > 0) { yield break; }

            var e = GameController.FlipCards(TurnTakerController.CharacterCards.Where(c => ! c.IsFlipped).Select(c => FindCardController(c)));
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            e = GameController.FlipCard(this, allowBackToFront: false);
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
