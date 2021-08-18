using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Armsmaster
{
    public class GrappleCardController : ModuleCardController
    {
        public GrappleCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator DoPrimary()
        {
            // "Armsmaster deals 1 projectile damage to up to 2 non-hero targets"
            var e = GameController.SelectTargetsAndDealDamage(
                HeroTurnTakerController,
                new DamageSource(GameController, CharacterCard),
                1,
                DamageType.Projectile,
                2,
                optional: false,
                null,
                additionalCriteria: c => !c.IsHero,
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

        public override IEnumerator DoSecondary()
        {
            // "Destroy an Ongoing card"
            var e = GameController.SelectAndDestroyCard(
                HeroTurnTakerController,
                new LinqCardCriteria(c => c.IsOngoing, "ongoing"),
                optional: false,
                responsibleCard: Card,
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
