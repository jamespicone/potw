using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Labyrinth
{
    public class CruelCityCardController : LabyrinthOneShotCardController
    {
        public CruelCityCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator UsualEffect()
        {
            // "Deal 5 damage of any type to a non-hero target.",
            var result = new List<SelectDamageTypeDecision>();
            var e = GameController.SelectDamageType(HeroTurnTakerController, result, cardSource: GetCardSource());
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            var damageType = GetSelectedDamageType(result);
            if (damageType != null)
            {
                e = GameController.SelectTargetsAndDealDamage(
                    HeroTurnTakerController,
                    new DamageSource(GameController, CharacterCard),
                    4,
                    damageType.Value,
                    numberOfTargets: 1,
                    optional: false,
                    requiredTargets: 1,
                    cardSource: GetCardSource()
                );
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }
        }

        public override IEnumerator ShapingDestroyEffect()
        {
            // destroy an Ongoing card
            var e = GameController.SelectAndDestroyCard(
                HeroTurnTakerController,
                new LinqCardCriteria(c => c.Is().Ongoing().AccordingTo(this), "ongoing"),
                optional: false,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }
    }
}
