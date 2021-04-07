using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            IEnumerator destroyWeaponCoroutine = base.GameController.SelectAndDestroyCard(base.HeroTurnTakerController, WeaponCard(), false, responsibleCard: base.Card, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(destroyWeaponCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(destroyWeaponCoroutine);
            }
            // "Destroy up to 2 Ongoing and/or environment cards."
            IEnumerator destroyOngEnvCoroutine = base.GameController.SelectAndDestroyCards(base.HeroTurnTakerController, new LinqCardCriteria((Card c) => c.IsOngoing || c.IsEnvironment, "ongoing or environment"), 2, false, 0, responsibleCard: base.Card, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(destroyOngEnvCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(destroyOngEnvCoroutine);
            }
            yield break;
        }
    }
}
