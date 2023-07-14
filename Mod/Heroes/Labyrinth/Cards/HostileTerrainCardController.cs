using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Labyrinth
{
    public class HostileTerrainCardController : LabyrinthOneShotCardController
    {
        public HostileTerrainCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator UsualEffect()
        {
            // "You may destroy a non-character-card target."
            var e = GameController.SelectAndDestroyCard(
                HeroTurnTakerController,
                new LinqCardCriteria(c => c.Is().Noncharacter().Target(), "non-character target", useCardsSuffix: false),
                optional: true,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }

        public override IEnumerator ShapingDestroyEffect()
        {
            // the environment deals each villain target 2 irreducible projectile damage.
            var e = GameController.SelectTargetsAndDealDamage(
                HeroTurnTakerController,
                new DamageSource(GameController, FindEnvironment().TurnTaker),
                2,
                DamageType.Projectile,
                numberOfTargets: null,
                optional: false,
                requiredTargets: null,
                isIrreducible: true,
                allowAutoDecide: true,
                additionalCriteria: c => c.Is().Villain().Target().AccordingTo(this),
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }
    }
}
