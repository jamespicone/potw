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

        new public bool MacheteActive => CanActivateEffect(base.TurnTakerController, MacheteIcon) || Journal.GetCardPropertiesBoolean(base.Card, ActivateAllIcons) == true;
        new public bool PistolActive => CanActivateEffect(base.TurnTakerController, PistolIcon) || Journal.GetCardPropertiesBoolean(base.Card, ActivateAllIcons) == true;
        new public bool SmgActive => CanActivateEffect(base.TurnTakerController, SmgIcon) || Journal.GetCardPropertiesBoolean(base.Card, ActivateAllIcons) == true;
        new public bool SniperActive => CanActivateEffect(base.TurnTakerController, SniperIcon) || Journal.GetCardPropertiesBoolean(base.Card, ActivateAllIcons) == true;
    }
}
