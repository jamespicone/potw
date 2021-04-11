using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.JessicaYamada
{
    public class JessicaYamadaCharacterCardController : HeroCharacterCardController
    {
        public JessicaYamadaCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            /*
                "1 player draws 1 card",
                "1 player plays 1 card",
                "1 player regains 2 HP"
             */

            IEnumerator e;
            switch(index)
            {
                case 0:
                    e = GameController.SelectHeroToDrawCard(
                        HeroTurnTakerController,
                        cardSource: GetCardSource()                        
                    );
                    break;

                case 1:
                    e = GameController.SelectHeroToPlayCard(
                        HeroTurnTakerController,
                        cardSource: GetCardSource()
                    );
                    break;

                case 2:
                    e = GameController.SelectAndGainHP(
                        HeroTurnTakerController,
                        2,
                        cardSource: GetCardSource()
                    );
                    break;

                default: yield break;
            }

            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            };
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

        public override bool AskIfActionCanBePerformed(GameAction ga)
        {
            var ua = ga as UnincapacitateHeroAction;
            if (ua == null) { return true; }

            if (ua.HeroCharacterCard != this) { return true; }
            if (Card.IsFlipped) { return true; }

            return false;
        }
    }
}
