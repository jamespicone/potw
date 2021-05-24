using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Slaughterhouse9
{
    public class ShallWePlayAGameCardController : CardController
    {
        public ShallWePlayAGameCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            var jack = FindCard("JackSlashCharacter");
            if (jack == null) { yield break; }

            var nineCard = FindCard("Slaughterhouse9Character", realCardsOnly: false);
            if (nineCard == null) { yield break; }

            // "If Jack Slash is not in play, find Jack Slash under the Slaughterhouse 9 card and put him into the villain play area.
            if (jack.Location == nineCard.UnderLocation)
            {
                var e = GameController.PlayCard(
                    TurnTakerController,
                    jack,
                    isPutIntoPlay: true,
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

            // Jack Slash deals 2 psychic damage to all hero targets
            if (! jack.IsInPlayAndHasGameText) { yield break; }
            var e2 = DealDamage(
                jack,
                c => c.IsHero && c.IsTarget,
                2,
                DamageType.Psychic
            );

            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e2);
            }
            else
            {
                GameController.ExhaustCoroutine(e2);
            }
        }
    }
}
