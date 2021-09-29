using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.Tattletale
{
    public class SpotTheFlawCardController : CardController
    {
        public SpotTheFlawCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // "Damage dealt by hero targets is irreducible."
            AddMakeDamageIrreducibleTrigger((DealDamageAction dda) => dda.DamageSource.IsHero);
            // "At the start of your turn, destroy this card."
            AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => base.GameController.DestroyCard(base.HeroTurnTakerController, base.Card, responsibleCard: base.Card, cardSource: GetCardSource()), TriggerType.DestroySelf);
            base.AddTriggers();
        }

        public override IEnumerator Play()
        {
            // "When this card enters play, {TattletaleCharacter} deals herself 2 psychic damage."
            IEnumerator damageCoroutine = base.DealDamage(base.CharacterCard, base.CharacterCard, 2, DamageType.Psychic, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(damageCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(damageCoroutine);
            }
            yield break;
        }
    }
}
