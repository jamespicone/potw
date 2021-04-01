using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Tattletale
{
    public class TattletaleCharacterCardController : HeroCharacterCardController
    {
        public TattletaleCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override IEnumerator UsePower(int index = 0)
        {
            // "1 player draws 2 cards, then discards 1 card."
            int numPlayers = GetPowerNumeral(0, 1);
            int numDraws = GetPowerNumeral(1, 2);
            int numDiscards = GetPowerNumeral(2, 1);
            // TODO: Problem:
            // If I use SelectTurnTakersAndDoAction, I can't pass the power numeral values to the function that actually does the drawing and discarding
            // If I use a for loop (with a List<TurnTaker> to prevent repeats) to iterate over the correct number of unique players, then it won't say "select the [first/second] player to..."
            //IEnumerator selectCoroutine = base.GameController.SelectTurnTakersAndDoAction(base.HeroTurnTakerController, new LinqTurnTakerCriteria((TurnTaker tt) => tt.IsHero && !tt.IsIncapacitatedOrOutOfGame), )
            // ...
            yield break;
        }

        public IEnumerator DrawDiscardResponse(TurnTaker tt, int cardsToDraw, int cardsToDiscard)
        {
            // Chosen player draws cards
            // ...
            // Chosen player discards cards
            // ...
            yield break;
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator incapCoroutine;
            switch (index)
            {
                case 0:
                    incapCoroutine = UseIncapOption1();
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(incapCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(incapCoroutine);
                    }
                    break;
                case 1:
                    incapCoroutine = UseIncapOption2();
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(incapCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(incapCoroutine);
                    }
                    break;
                case 2:
                    incapCoroutine = UseIncapOption3();
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(incapCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(incapCoroutine);
                    }
                    break;
            }
            yield break;
        }

        private IEnumerator UseIncapOption1()
        {
            yield break;
        }

        private IEnumerator UseIncapOption2()
        {
            yield break;
        }

        private IEnumerator UseIncapOption3()
        {
            yield break;
        }
    }
}
