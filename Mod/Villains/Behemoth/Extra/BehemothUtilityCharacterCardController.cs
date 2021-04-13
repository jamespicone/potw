using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Behemoth
{
    public class BehemothUtilityCharacterCardController : VillainCharacterCardController
    {
        public BehemothUtilityCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public const string ProximityMarkerIdentifier = "Proximity";
        public const string ProximityPoolIdentifier = "ProximityPool";

        public const string MovementDeckIdentifier = "MovementDeck";
        public const string HeroTacticsIdentifier = "HeroTacticsCharacter";

        public TokenPool ProximityPool(TurnTaker tt)
        {
            if (tt != null && tt.IsHero && !tt.IsIncapacitatedOrOutOfGame)
            {
                Card proximityMarker = base.GameController.FindCardsWhere(new LinqCardCriteria((Card c) => c.Identifier == ProximityMarkerIdentifier && c.Location == tt.PlayArea), realCardsOnly: false).FirstOrDefault();
                if (proximityMarker != null)
                {
                    TokenPool proximityPool = proximityMarker.FindTokenPool(ProximityPoolIdentifier);
                    return proximityPool;
                }
            }
            return null;
        }

        public TurnTaker TurnTakerForPool(TokenPool proximityPool)
        {
            if (proximityPool.Identifier == ProximityPoolIdentifier)
            {
                Card proximityMarker = proximityPool.CardWithTokenPool;
                if (proximityMarker.Identifier == ProximityMarkerIdentifier && proximityMarker.IsInPlayAndHasGameText)
                {
                    return proximityMarker.Location.HighestRecursiveLocation.OwnerTurnTaker;
                }
            }
            return null;
        }

        public int? ProximityTokens(Card c)
        {
            if (c != null && c.IsHero)
            {
                TurnTaker player = c.Owner;
                TokenPool proximity = ProximityPool(player);
                if (proximity != null)
                {
                    return proximity.CurrentValue;
                }
            }
            return 0;
        }

        public bool AllProximityPoolsEmpty()
        {
            IEnumerable<HeroTurnTakerController> heroControllers = base.GameController.FindHeroTurnTakerControllers();
            bool allEmpty = true;
            foreach (HeroTurnTakerController player in heroControllers)
            {
                if (!player.IsIncapacitatedOrOutOfGame)
                {
                    TokenPool playerProximity = ProximityPool(player.TurnTaker);
                    if (playerProximity != null)
                    {
                        allEmpty &= playerProximity.CurrentValue == 0;
                    }
                }
            }
            return allEmpty;
        }
    }
}
