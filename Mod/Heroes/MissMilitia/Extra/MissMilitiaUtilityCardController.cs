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
        public MissMilitiaUtilityCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public string ShowWeaponStatus(WeaponType type)
        {
            if (this.ShouldActivateWeaponAbility(type))
            {
                return $"[b]{MissMilitiaExtensions.HumanReadableName(type)}[/b] effects will activate";
            }
            else
            {
                return $"[b]{MissMilitiaExtensions.HumanReadableName(type)}[/b] effects will not activate";
            }
        }

        public void ShowWeaponStatusIfActive(WeaponType type)
        {
            SpecialStringMaker.ShowSpecialString(() => ShowWeaponStatus(type)).Condition = () => this.ShouldActivateWeaponAbility(type);
        }

        public string UsedWeaponList()
        {
            List<string> usedWeapons = new List<string>();
            foreach (WeaponType type in Enum.GetValues(typeof(WeaponType)))
            {
                if (this.ShouldActivateWeaponAbility(type))
                {
                    usedWeapons.Add(MissMilitiaExtensions.HumanReadableName(type));
                }
            }

            if (usedWeapons.Count() > 0)
            {
                if (usedWeapons.Count() > 1)
                {
                    return $"{TurnTaker.Name}'s weapon effects that will activate: {usedWeapons.ToCommaList()}.";
                }
                else
                {
                    return $"{TurnTaker.Name}'s only weapon effect that will activate is {usedWeapons.First()}.";
                }
            }
            else
            {
                return $"None of {TurnTaker.Name}'s weapon effects will activate.";
            }
        }
    }
}
