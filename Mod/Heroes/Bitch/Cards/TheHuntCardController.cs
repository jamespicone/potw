using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Bitch
{
    public class TheHuntCardController : CardController
    {
        public TheHuntCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.DoKeywordsContain("dog"), "dog"));
        }

        public override void AddTriggers()
        {
            AddStartOfTurnTrigger(turntaker => turntaker == TurnTaker, action => Act(), TriggerType.DealDamage);
        }

        public System.Collections.IEnumerator Act()
        {
            var cards = GameController.FindCardsWhere(card => card.IsInPlay && card.DoKeywordsContain("dog"), true, GetCardSource()).ToList();

            while (cards.Count > 0)
            {
                var storedResults = new List<SelectCardDecision>();
                var e = GameController.SelectCardAndStoreResults(
                    HeroTurnTakerController,
                    SelectionType.CardToDealDamage,
                    new LinqCardCriteria(c => cards.Contains(c)),
                    storedResults,
                    optional: false,
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

                var selectedDog = storedResults.FirstOrDefault();
                if (selectedDog == null) { break; }

                cards.Remove(selectedDog.SelectedCard);

                e = GameController.SelectTargetsAndDealDamage(
                    HeroTurnTakerController,
                    new DamageSource(GameController, selectedDog.SelectedCard),
                    1,
                    DamageType.Melee,
                    1,
                    optional: false,
                    requiredTargets: 0,
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
        }

        public override System.Collections.IEnumerator Play()
        {
            // At the start of your turn each Dog in play may deal 1 melee damage to a target
            yield break;
        }
    }
}
