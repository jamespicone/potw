using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.MissMilitia
{
    public class WeaponCardController : MissMilitiaUtilityCardController
    {
        protected string EffectIcon
        {
            get;
            private set;
        }

        public static readonly string ActivateAllIcons = "ActivateAllIcons";

        public WeaponCardController(Card card, TurnTakerController turnTakerController, string icon)
            : base(card, turnTakerController)
        {
            AddThisCardControllerToList(CardControllerListType.ActivatesEffects);
            EffectIcon = icon;
        }

        public override bool? AskIfActivatesEffect(TurnTakerController turnTakerController, string effectKey)
        {
            bool? result = null;
            if (turnTakerController == base.TurnTakerController && effectKey == EffectIcon)
            {
                result = true;
            }
            return result;
        }

        public bool ActivateWeaponEffectForPower(string weaponKey)
        {
            return HasUsedWeaponSinceStartOfLastTurn(weaponKey) || Journal.GetCardPropertiesBoolean(base.Card, ActivateAllIcons) == true;
        }
    }
}
