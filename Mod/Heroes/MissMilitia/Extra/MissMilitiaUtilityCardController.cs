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
    public class MissMilitiaUtilityCardController : CardController
    {
        public const string MacheteIcon = "{machete}";
        public const string PistolIcon = "{pistol}";
        public const string SmgIcon = "{smg}";
        public const string SniperIcon = "{sniper}";
        public string[] AllIcons = { MacheteIcon, PistolIcon, SmgIcon, SniperIcon };

        public bool MacheteActive => CanActivateEffect(base.TurnTakerController, MacheteIcon);
        public bool PistolActive => CanActivateEffect(base.TurnTakerController, PistolIcon);
        public bool SmgActive => CanActivateEffect(base.TurnTakerController, SmgIcon);
        public bool SniperActive => CanActivateEffect(base.TurnTakerController, SniperIcon);

        public MissMilitiaUtilityCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public bool IconStatus(string iconKey)
        {
            if (AllIcons.Contains(iconKey))
            {
                if (iconKey == MacheteIcon)
                {
                    return MacheteActive;
                }
                if (iconKey == PistolIcon)
                {
                    return PistolActive;
                }
                if (iconKey == SmgIcon)
                {
                    return SmgActive;
                }
                if (iconKey == SniperIcon)
                {
                    return SniperActive;
                }
            }
            return false;
        }

        public string ShowIconStatus(string iconKey)
        {
            if (AllIcons.Contains(iconKey))
            {
                bool active = IconStatus(iconKey);
                if (active)
                {
                    return iconKey + " is active.";
                }
                else
                {
                    return iconKey + " is not active.";
                }
            }
            return "";
        }

        public void ShowIconStatusIfActive(string iconKey)
        {
            SpecialStringMaker.ShowSpecialString(() => ShowIconStatus(iconKey)).Condition = () => IconStatus(iconKey);
        }

        public string ActiveIconList()
        {
            List<string> icons = new List<string>();
            if (MacheteActive)
            {
                icons.Add(MacheteIcon);
            }
            if (PistolActive)
            {
                icons.Add(PistolIcon);
            }
            if (SmgActive)
            {
                icons.Add(SmgIcon);
            }
            if (SniperActive)
            {
                icons.Add(SniperIcon);
            }

            if (icons.Count() > 0)
            {
                return base.TurnTaker.Name + "'s current active Weapon " + icons.Count().ToString_SingularOrPlural("effect", "effects") + ": " + icons.ToCommaList() + ".";
            }
            else
            {
                return base.TurnTaker.Name + " does not have any active Weapon effects.";
            }
        }
    }
}
