using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Alexandria
{
    public class AlexandriasCapeCardController : CardController
    {
        public AlexandriasCapeCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator UsePower(int index = 0)
        {
            // {AlexandriaCharacter} deals 3 melee damage to a target.
            var e = GameController.SelectTargetsAndDealDamage(
                HeroTurnTakerController,
                new DamageSource(GameController, CharacterCard),
                amount: GetPowerNumeral(0, 3),
                DamageType.Melee,
                numberOfTargets: 1,
                optional: false,
                requiredTargets: 1,
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

            // Return one of your noncharacter cards in play to your hand
            e = GameController.SelectAndReturnCards(
                 HeroTurnTakerController,
                 1,
                 new LinqCardCriteria(
                     c => c.IsInPlay &&
                         c.Location == TurnTaker.PlayArea &&
                         c.Owner == TurnTaker &&
                         !c.IsCharacter &&
                         !c.IsOneShot &&
                         !GameController.IsCardIndestructible(c)
                 ),
                 toHand: true,
                 toDeck: false,
                 optional: false,
                 requiredDecisions: 1,
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
