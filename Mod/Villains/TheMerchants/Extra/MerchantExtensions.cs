using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Jp.ParahumansOfTheWormverse.TheMerchants
{
    public static class MerchantsExtensions
    {
        public static Location GetThugDeck(this TurnTaker turnTaker)
        {
            return turnTaker.FindSubDeck("ThugDeck");
        }

        public static IEnumerator PlayThugs(this CardController cc, int number = 1)
        {
            Log.Debug($"PlayThugs {number}");
            return cc.TurnTakerController.GameController.PlayTopCardOfLocation(
                cc.TurnTakerController,
                cc.TurnTaker.GetThugDeck(),
                numberOfCards: number,
                cardSource: cc.GetCardSource()
            );
        }
    }
}
