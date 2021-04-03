using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Battery
{
    public class BatteryUtilityCharacterCardController : HeroCharacterCardController
    {
        public const string ChargedIdentifier = "ChargedIndicator";
        public int DischargePowerIndex
        {
            get;
            protected set;
        }

        public BatteryUtilityCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            ShowBatteryChargedStatus();
        }

        protected void ShowChargedStatus(Card c)
        {
            if (c.IsInPlay && (c.IsActive || !c.IsCharacter))
            {
                SpecialStringMaker.ShowIfElseSpecialString(() => IsCharged(c), () => c.Identifier + " is Charged.", () => c.Identifier + " is Discharged.");
            }
        }

        protected void ShowBatteryChargedStatus()
        {
            ShowChargedStatus(base.CharacterCard);
        }

        protected bool IsByChargedIndicator(Card c)
        {
            return c.NextToLocation.Cards.Any((Card nextTo) => nextTo.Identifier == ChargedIdentifier);
        }

        protected bool IsCharged(Card c)
        {
            if (c.IsActive)
            {
                return IsByChargedIndicator(c);
            }
            else
            {
                return false;
            }
        }

        protected bool IsBatteryCharged()
        {
            return IsCharged(base.CharacterCard);
        }

        protected IEnumerator Charge(Card toCharge)
        {
            if (IsCharged(toCharge))
            {
                yield break;
            }
            Card indicator = base.TurnTaker.GetAllCards(realCardsOnly: false).Where((Card c) => !c.IsRealCard && c.Location.IsOffToTheSide && c.Identifier == ChargedIdentifier).FirstOrDefault();
            if (indicator != null)
            {
                IEnumerator moveCoroutine = base.GameController.MoveCard(base.TurnTakerController, indicator, toCharge.NextToLocation, doesNotEnterPlay: true, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(moveCoroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(moveCoroutine);
                }
            }
            yield break;
        }

        protected IEnumerator RemoveCharge(Card toDischarge)
        {
            IEnumerable<Card> indicators = toDischarge?.NextToLocation.Cards.Where((Card c) => !c.IsRealCard && c.Identifier == ChargedIdentifier);
            if (indicators != null && indicators.Any())
            {
                IEnumerator removeCoroutine = BulkMoveCard(base.TurnTakerController, indicators, base.TurnTaker.OffToTheSide, false, false, base.TurnTakerController, false);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(removeCoroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(removeCoroutine);
                }
            }
            yield break;
        }

        protected IEnumerator ChargeCharacter()
        {
            return Charge(base.CharacterCard);
        }

        protected IEnumerator DischargeCharacter()
        {
            return RemoveCharge(base.CharacterCard);
        }
    }
}
