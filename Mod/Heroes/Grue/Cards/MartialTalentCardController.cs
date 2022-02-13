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
    public class MartialTalentCardController : CardController
    {
        public MartialTalentCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddTriggers()
        {
            // Increase melee damage dealt by {GrueCharacter} by 1
            AddIncreaseDamageTrigger(dda => dda.DamageSource.Card == CharacterCard && dda.DamageType == DamageType.Melee, 1);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // {GrueCharacter} deals a target 2 melee damage
            return GameController.SelectTargetsAndDealDamage(
                HeroTurnTakerController,
                new DamageSource(GameController, CharacterCard),
                GetPowerNumeral(0, 2),
                DamageType.Melee,
                numberOfTargets: 1,
                optional: false,
                requiredTargets: 1,
                cardSource: GetCardSource()
            );
        }
    }
}
