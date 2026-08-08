using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.TheSimurgh
{
    public class AFateSelectedCardController : CardController, ISimurghDangerCard
    {
        public AFateSelectedCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public int Danger()
        {
            return 7;
        }

        public override IEnumerator Play()
        {
            // {TheSimurghCharacter} deals the hero with the fewest cards in play {H} sonic damage.
            // (Note DealDamageToMostCardsInPlay can't be used here: its mostFewestSelectionType
            // parameter only changes the decision label, not the most/fewest logic.)
            var storedResults = new List<TurnTaker>();
            var e = FindHeroWithFewestCardsInPlay(storedResults, evenIfCannotDealDamage: true);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            var victim = storedResults.FirstOrDefault();
            if (victim == null) { yield break; }

            var characterResults = new List<Card>();
            e = FindCharacterCardToTakeDamage(victim, characterResults, CharacterCard, H, DamageType.Sonic);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            var target = characterResults.FirstOrDefault();
            if (target == null) { yield break; }

            e = DealDamage(CharacterCard, target, H, DamageType.Sonic, cardSource: GetCardSource());
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
