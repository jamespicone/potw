using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Legend
{
    public class EnergyConversionCardController : CardController
    {
        public EnergyConversionCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            // "Whenever Legend is dealt damage by a villain or environment target, you may draw a card or play a card"
            AddTrigger<DealDamageAction>(
                dda => dda.DidDealDamage && dda.Target == CharacterCard && (dda.DamageSource.Is().Environment().Target() || dda.DamageSource.Is(this).Villain().Target()),
                dda => DrawACardOrPlayACard(HeroTurnTakerController, optional: true),
                new TriggerType[] { TriggerType.DrawCard, TriggerType.PlayCard },
                TriggerTiming.After
            );
        }
    }
}
