using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

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
            List<SelectTurnTakerDecision> storedDecisions = new List<SelectTurnTakerDecision>();
            List<SelectTurnTakerDecision> storedHeroes = new List<SelectTurnTakerDecision>();
            for (int i = 0; i < numPlayers; i++)
            {
                // Select a player
                IEnumerator chooseCoroutine = base.GameController.SelectHeroTurnTaker(base.HeroTurnTakerController, SelectionType.DrawCard, false, false, storedDecisions, new LinqTurnTakerCriteria((TurnTaker tt) => !storedDecisions.Select((SelectTurnTakerDecision sttd) => sttd.SelectedTurnTaker).Contains(tt)), numDraws, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(chooseCoroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(chooseCoroutine);
                }
                // That player draws and discards
                TurnTaker chosen = (from d in storedDecisions where d.Completed select d.SelectedTurnTaker).FirstOrDefault();
                if (chosen != null && chosen.Is(this).Hero() && !chosen.IsIncapacitatedOrOutOfGame)
                {
                    IEnumerator drawDiscardCoroutine = DrawDiscardResponse(chosen, numDraws, numDiscards);
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(drawDiscardCoroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(drawDiscardCoroutine);
                    }
                }
                storedHeroes = new List<SelectTurnTakerDecision>(storedDecisions);
                storedDecisions.Clear();
            }
            yield break;
        }

        public IEnumerator DrawDiscardResponse(TurnTaker tt, int cardsToDraw, int cardsToDiscard)
        {
            // Chosen player draws cards
            IEnumerator drawCoroutine = base.GameController.DrawCards(base.GameController.FindHeroTurnTakerController(tt.ToHero()), cardsToDraw, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(drawCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(drawCoroutine);
            }
            // Chosen player discards cards
            IEnumerator discardCoroutine = base.GameController.SelectAndDiscardCards(base.GameController.FindHeroTurnTakerController(tt.ToHero()), cardsToDiscard, false, cardsToDiscard, responsibleTurnTaker: base.TurnTaker, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(discardCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(discardCoroutine);
            }
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
            // "One player draws a card."
            IEnumerator drawCoroutine = base.GameController.SelectHeroToDrawCard(base.HeroTurnTakerController, optionalDrawCard: false, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(drawCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(drawCoroutine);
            }
            yield break;
        }

        private IEnumerator UseIncapOption2()
        {
            // "One player plays a card."
            IEnumerator playCoroutine = base.GameController.SelectHeroToPlayCard(base.HeroTurnTakerController, optionalPlayCard: false, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(playCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(playCoroutine);
            }
            yield break;
        }

        private IEnumerator UseIncapOption3()
        {
            // "Destroy an Ongoing card."
            IEnumerator destroyCoroutine = base.GameController.SelectAndDestroyCard(base.HeroTurnTakerController, new LinqCardCriteria((Card c) => c.DoKeywordsContain("ongoing")), false, responsibleCard: base.Card, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(destroyCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(destroyCoroutine);
            }
            yield break;
        }
    }
}
