using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Grue
{
    public class SkullHelmetCardController : CardController
    {
        public SkullHelmetCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            // Whenever a Darkness card is placed next to a target, {GrueCharacter} may deal that target 1 psychic damage
            AddTrigger<PlayCardAction>(
                pca => pca.CardToPlay.IsGrueDarkness() && pca.IsSuccessful && pca.OverridePlayLocation.IsNextToCard,
                pca => MaybeHurtTarget(pca),
                TriggerType.DealDamage,
                TriggerTiming.After
            );
        }

        private IEnumerator MaybeHurtTarget(PlayCardAction pca)
        {
            return DealDamage(
                CharacterCard,
                pca.OverridePlayLocation.OwnerCard,
                1,
                DamageType.Psychic,
                optional: true,
                cardSource: GetCardSource()
            );
        }
    }
}
