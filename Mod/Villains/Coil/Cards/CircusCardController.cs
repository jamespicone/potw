using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.Coil
{
    public class CircusCardController : CardController
    {
        public CircusCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // Whenever a hero card is played this card deals the hero who played the card 1 projectile damage
            AddTrigger<PlayCardAction>(
                pca => this.HasAlignment(pca.CardToPlay, CardAlignment.Hero) && pca.WasCardPlayed && this.HasAlignment(pca.TurnTakerController.TurnTaker, CardAlignment.Hero) && !pca.IsPutIntoPlay,
                pca => HurtHero(pca),
                TriggerType.DealDamage,
                TriggerTiming.After
            );
        }

        private IEnumerator HurtHero(PlayCardAction pca)
        {
            var httc = pca.TurnTakerController as HeroTurnTakerController;
            if (httc == null) { yield break; }

            var storedResults = new List<Card>();
            var e = FindCharacterCardToTakeDamage(
                httc.TurnTaker,
                storedResults,
                Card,
                1,
                DamageType.Projectile
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            var cardToDamage = storedResults.FirstOrDefault();
            if (cardToDamage == null) { yield break; }

            e = DealDamage(
                Card,
                cardToDamage,
                1,
                DamageType.Projectile,
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
