using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Armsmaster
{
    public class StaticFieldGeneratorCardController : ModuleCardController
    {
        public StaticFieldGeneratorCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowListOfCardsInPlay(new LinqCardCriteria((Card c) => c.IsTarget && c.HitPoints.HasValue && c.HitPoints.Value <= 3, "targets with 3 or fewer HP", false, false, "target with 3 or fewer HP", "targets with 3 or fewer HP"));
        }

        public override IEnumerator DoPrimary()
        {
            // "Armsmaster deals 2 lightning damage to a non-hero target"
            var e = GameController.SelectTargetsAndDealDamage(
                HeroTurnTakerController,
                new DamageSource(GameController, Card),
                2,
                DamageType.Lightning,
                1,
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
            // Armsmaster deals 1 lightning damage to all targets with 3 or less HP
            var e = GameController.DealDamage(HeroTurnTakerController, CharacterCard, c => c.IsTarget && c.HitPoints <= 3, 1, DamageType.Lightning, cardSource: GetCardSource());
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
