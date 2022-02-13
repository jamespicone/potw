using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.MissMilitia
{
    public class IHaveMoreCardController : MissMilitiaUtilityCardController
    {
        public IHaveMoreCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowListOfCardsInPlay(WeaponCard());
        }

        public LinqCardCriteria WeaponCard()
        {
            return new LinqCardCriteria((Card c) => c.DoKeywordsContain("weapon"), "weapon");
        }

        public override IEnumerator Play()
        {
            // "Destroy a Weapon card."
            var e = GameController.SelectAndDestroyCard(
                HeroTurnTakerController,
                WeaponCard(),
                optional: false,
                responsibleCard: Card,
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

            // "Destroy up to 2 Ongoing and/or environment cards."
            e = GameController.SelectAndDestroyCards(
                HeroTurnTakerController,
                new LinqCardCriteria((c) => c.IsOngoing || c.Is().Environment(), "ongoing or environment"),
                numberOfCards: 2,
                optional: false,
                requiredDecisions: 0,
                responsibleCard: Card,
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
}
