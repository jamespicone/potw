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
    public class AnAmbushLaidCardController : SimurghPlayWhenRevealedCardController, ISimurghDangerCard
    {
        // "When this card is revealed, play it.",

        public AnAmbushLaidCardController(Card card, TurnTakerController controller) : base(card, controller)
        {}

        public int Danger()
        {
            return 6;
        }

        protected override string SurpriseMessage()
        {
            return "The Simurgh laid an ambush!";
        }

        private int DamageAmount()
        {
            return FindCardsWhere(c => ! c.IsFlipped && c.DoKeywordsContain("trap") && c.IsInPlay).Count() + 1;
        }

        public override IEnumerator Play()
        {
            // "{TheSimurghCharacter} deals each hero target X fire damage, where X is the number of face-up Trap cards plus 1."
            return DealDamage(
                CharacterCard,
                c => c.Is(this).Hero().Target(),
                c => DamageAmount(),
                DamageType.Fire
            );
        }
    }
}
