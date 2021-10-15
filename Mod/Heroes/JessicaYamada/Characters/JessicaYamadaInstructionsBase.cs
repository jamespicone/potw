using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.JessicaYamada
{
    public class JessicaYamadaInstructionsBase : HeroCharacterCardController
    {
        public JessicaYamadaInstructionsBase(Card card, TurnTakerController controller) : base(card, controller)
        {
        }

        public override void AddTriggers()
        {
            if (!Card.IsFlipped)
            {
                AddSideTrigger(AddCannotDealDamageTrigger(c => c == CharacterCard));

                AddSideTrigger(AddTrigger<GameAction>(
                    (ga) => ((ga is FlipCardAction || ga is BulkRemoveTargetsAction || ga is MoveCardAction) && !CharacterCard.IsFlipped),
                    (ga) => IncapacitateIfShouldBeIncapped(),
                    TriggerType.FirstTrigger,
                    TriggerTiming.After,
                    priority: TriggerPriority.High
                ));

                AddSideTrigger(AddTrigger<GameAction>(
                    (ga) => ((ga is FlipCardAction || ga is BulkRemoveTargetsAction || ga is MoveCardAction) && CharacterCard.IsFlipped),
                    (ga) => IncapInstructionCard(),
                    TriggerType.FirstTrigger,
                    TriggerTiming.After,
                    priority: TriggerPriority.High
                ));
            }
            else
            {
                AddSideTrigger(AddTrigger<GameAction>(
                    (ga) => ((ga is FlipCardAction || ga is BulkRemoveTargetsAction || ga is MoveCardAction || ga is UnincapacitateHeroAction) && !CharacterCard.IsFlipped),
                    (ga) => ReviveInstructionCard(),
                    TriggerType.FirstTrigger,
                    TriggerTiming.After,
                    priority: TriggerPriority.High
                ));
            }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // Another player either regains 2 HP or draws a card
            var selectedPlayerList = new List<SelectTurnTakerDecision>();
            var e = GameController.SelectHeroTurnTaker(
                HeroTurnTakerController,
                SelectionType.AmbiguousDecision,
                optional: false,
                allowAutoDecide: false,
                storedResults: selectedPlayerList,
                heroCriteria: new LinqTurnTakerCriteria(tt => tt != TurnTaker),
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

            var selectedPlayer = GetSelectedTurnTaker(selectedPlayerList);
            if (selectedPlayer == null) { yield break; }

            var selectedPlayerController = FindTurnTakerController(selectedPlayer) as HeroTurnTakerController;
            if (selectedPlayerController == null) { yield break; }

            e = SelectAndPerformFunction(
                selectedPlayerController,
                new[] {
                    new Function(selectedPlayerController, "Regain 2 HP", SelectionType.GainHP, () => RegainHPFunc(selectedPlayerController)),
                    new Function(selectedPlayerController, "Draw a card", SelectionType.DrawCard, () => DrawFunc(selectedPlayerController))
                },
                associatedCards: new[] { Card }
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

        private IEnumerator RegainHPFunc(HeroTurnTakerController ttc)
        {
            var e = GameController.SelectAndGainHP(
                ttc,
                GetPowerNumeral(0, 2),
                additionalCriteria: c => ttc.CharacterCards.Contains(c),
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

        private IEnumerator DrawFunc(HeroTurnTakerController ttc)
        {
            var e = DrawCard(ttc.HeroTurnTaker);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        public IEnumerator IncapacitateIfShouldBeIncapped()
        {
            if (TurnTaker.IsIncapacitatedOrOutOfGame) {
                yield break;
            }

            if (GameController.FindTargetsInPlay(c => this.HasAlignment(c, CardAlignment.Hero, CardTarget.Target) && c.Location != TurnTaker.PlayArea).Count() > 0) { yield break; }

            var e = GameController.DestroyCard(null, CharacterCard);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            e = GameController.DestroyCard(null, Card);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        public IEnumerator IncapInstructionCard()
        {
            var e = GameController.DestroyCard(null, Card);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        public IEnumerator ReviveInstructionCard()
        {
            var e = GameController.FlipCard(this, cardSource: GetCardSource());
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
