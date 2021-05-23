using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Slaughterhouse9
{
    public class SomeLightEntertainmentCardController : CardController
    {
        public SomeLightEntertainmentCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            // Each villain target in play deals 2 melee damage to the hero target with the highest HP
            List<Card> attacked = new List<Card>();
            while (true)
            {
                var villains = GameController.FindCardsWhere(card => card.IsInPlay && card.IsVillainTarget, true, GetCardSource()).Except(attacked);
                if (villains.Count() <= 0) { break; }

                var storedResults = new List<SelectCardDecision>();
                var e = GameController.SelectCardAndStoreResults(
                    DecisionMaker,
                    SelectionType.CardToDealDamage,
                    new LinqCardCriteria(c => villains.Contains(c)),
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

                var selectedVillain = storedResults.FirstOrDefault();
                if (selectedVillain == null) { break; }

                attacked.Add(selectedVillain.SelectedCard);

                e = DealDamageToHighestHP(
                    selectedVillain.SelectedCard,
                    1,
                    c => c.IsHero && c.IsTarget,
                    c => 2,
                    DamageType.Melee
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
    }
}
