using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.Battery
{
    public class MagnetismCardController : BatteryUtilityCardController
    {
        public MagnetismCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            ShowBatteryChargedStatus();
            base.SpecialStringMaker.ShowNumberOfCardsAtLocations(() => from httc in base.GameController.FindHeroTurnTakerControllers()
                                                                       where !httc.IsIncapacitatedOrOutOfGame
                                                                       select httc.TurnTaker.Trash, new LinqCardCriteria((Card c) => IsEquipment(c), "equipment"));
        }

        public override void AddTriggers()
        {
            base.AddTriggers();
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // "If {BatteryCharacter} is {Charged}, destroy a non-character card Equipment or Device card."
            if (IsBatteryCharged())
            {
                IEnumerator destroyCoroutine = base.GameController.SelectAndDestroyCard(base.HeroTurnTakerController, new LinqCardCriteria((Card c) => !c.IsCharacter && (c.DoKeywordsContain("equipment") || c.DoKeywordsContain("device")), "non-character Equipment or Device"), false, responsibleCard: base.Card, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(destroyCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(destroyCoroutine);
                }
            }
            // "If {BatteryCharacter} is {Discharged}, put an Equipment card from a hero trash into play."
            if (!IsBatteryCharged())
            {
                // TODO: This should be cards from a hero trash, not hero cards from a trash
                IEnumerator playCoroutine = base.GameController.SelectAndPlayCard(base.HeroTurnTakerController, (Card c) => c.DoKeywordsContain("equipment") && this.HasAlignment(c, CardAlignment.Hero) && c.IsInTrash, false, true, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(playCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(playCoroutine);
                }
            }
            yield break;
        }
    }
}
