using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Armsmaster
{
    public static class HalberdExtensions
    {
        public static IEnumerator DoHalberdAction(this CardController co)
        {
            var e = co.GameController.SelectAndActivateAbility(
                co.HeroTurnTakerController,
                "primary",
                new LinqCardCriteria(c => c.DoKeywordsContain("module") && co.Card.GetAllNextToCards(false).Contains(c) && ModuleCardController.IsPrimaryModule(co.GameController, c)),
                optional: true,
                cardSource: co.GetCardSource()
            );
            if (co.UseUnityCoroutines)
            {
                yield return co.GameController.StartCoroutine(e);
            }
            else
            {
                co.GameController.ExhaustCoroutine(e);
            }

            e = co.GameController.SelectAndActivateAbility(
                co.HeroTurnTakerController,
                "secondary",
                new LinqCardCriteria(c => c.DoKeywordsContain("module") && co.Card.GetAllNextToCards(false).Contains(c) && ModuleCardController.IsSecondaryModule(co.GameController, c)),
                optional: true,
                cardSource: co.GetCardSource()
            );
            if (co.UseUnityCoroutines)
            {
                yield return co.GameController.StartCoroutine(e);
            }
            else
            {
                co.GameController.ExhaustCoroutine(e);
            }
        }
    }
}
