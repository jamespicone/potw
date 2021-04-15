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
    public class BehemothTurnTakerController : TurnTakerController
    {
        public BehemothTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {

        }

        public override IEnumerator StartGame()
        {
            Log.Debug("BehemothTurnTakerController.StartGame activated.");
            /*// Put Hero Tactics into play
            Card heroTactics = base.TurnTaker.FindCard(HeroTacticsIdentifier);
            IEnumerator playCoroutine = base.GameController.PlayCard(this, heroTactics, isPutIntoPlay: true, responsibleTurnTaker: base.TurnTaker, cardSource: new CardSource(base.CharacterCardController));
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(playCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(playCoroutine);
            }*/
            // Put all Movement cards under Behemoth and shuffle them
            IEnumerable<Card> movements = base.GameController.FindCardsWhere(new LinqCardCriteria((Card c) => c.Owner == base.TurnTaker && c.DoKeywordsContain("movement"), "movement"));
            IEnumerator deckCoroutine = base.GameController.BulkMoveCards(this, movements, base.CharacterCard.UnderLocation, responsibleTurnTaker: base.TurnTaker, cardSource: CharacterCardController.GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(deckCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(deckCoroutine);
            }
            // Put a Proximity marker in each hero play area
            IEnumerable<HeroTurnTakerController> heroControllers = base.GameController.FindHeroTurnTakerControllers();
            foreach(HeroTurnTakerController player in heroControllers)
            {
                //Log.Debug("Finding a marker to move...");
                IEnumerable<Card> unassignedMarkers = base.GameController.FindCardsWhere((Card c) => c.Owner == base.TurnTaker && c.Identifier == ProximityMarkerIdentifier && c.IsOffToTheSide, realCardsOnly: false);
                //Log.Debug("unassignedMarkers.Count(): " + unassignedMarkers.Count().ToString());
                Card marker = unassignedMarkers.FirstOrDefault();
                //Log.Debug("Trying to move " + marker.Title + " to " + player.Name + "'s play area...");
                IEnumerator assignCoroutine = base.GameController.PlayCard(this, marker, isPutIntoPlay: true, overridePlayLocation: player.TurnTaker.PlayArea, responsibleTurnTaker: base.TurnTaker, canBeCancelled: false, cardSource: CharacterCardController.GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(assignCoroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(assignCoroutine);
                }
            }
            // Shuffle Movement deck
            IEnumerator shuffleMovementCoroutine = base.GameController.ShuffleLocation(base.CharacterCard.UnderLocation, cardSource: CharacterCardController.GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(shuffleMovementCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(shuffleMovementCoroutine);
            }
            // Shuffle villain deck
            IEnumerator shuffleCoroutine = base.GameController.ShuffleLocation(base.TurnTaker.Deck, cardSource: CharacterCardController.GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(shuffleCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(shuffleCoroutine);
            }
            Log.Debug("BehemothTurnTakerController.StartGame: shuffleCoroutine completed");
            // Make sure Behemoth has a damage type, even if it'll get changed before it's used
            BehemothCharacterCardController behemoth = (CharacterCardController as BehemothCharacterCardController);
            IEnumerator psychicCoroutine = behemoth.SetDamageType(null, DamageType.Psychic);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(psychicCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(psychicCoroutine);
            }
            Log.Debug("BehemothTurnTakerController.StartGame completed");
            yield break;
        }

        public const string MovementDeckIdentifier = "MovementDeck";
        public const string HeroTacticsIdentifier = "HeroTacticsCharacter";

        public const string ProximityMarkerIdentifier = "Proximity";
        public const string ProximityPoolIdentifier = "ProximityPool";
    }
}
