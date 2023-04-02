using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Echidna
{
    public class ApocryphaTwistedCardController : CardController
    {
        public ApocryphaTwistedCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowHeroCharacterCardWithHighestHP();
            SpecialStringMaker.ShowHeroWithMostCards(inHand: true);
        }

        public override void AddTriggers()
        {
            AddDealDamageAtEndOfTurnTrigger(
                TurnTaker,
                Card,
                c => c.Is(this).Hero().Character(),
                TargetType.HighestHP,
                2,
                DamageType.Melee
            );

            AddTrigger<DealDamageAction>(
                dda => dda.Target == CharacterCard && ! HasBeenSetToTrueThisRound("AlexandriaDamagePrevention"),
                dda => PreventDamage(dda),
                TriggerType.ImmuneToDamage,
                TriggerTiming.Before
            );             
        }

        private IEnumerator PreventDamage(DealDamageAction dda)
        {
            SetCardPropertyToTrueIfRealAction("AlexandriaDamagePrevention");
            return GameController.ImmuneToDamage(dda, GetCardSource());
        }
    }
}
