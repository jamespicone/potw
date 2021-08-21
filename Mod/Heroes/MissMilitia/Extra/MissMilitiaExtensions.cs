using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Jp.ParahumansOfTheWormverse.MissMilitia
{
    public enum WeaponType
    {
        Pistol,
        Machete,
        SubmachineGun,
        SniperRifle
    };

    public static class MissMilitiaExtensions
    {
        public static string EffectKey(WeaponType type)
        {
            switch(type)
            {
                case WeaponType.Pistol: return "{pistol}";
                case WeaponType.Machete: return "{machete}";
                case WeaponType.SubmachineGun: return "{smg}";
                case WeaponType.SniperRifle: return "{sniper}";
            }

            throw new ArgumentException($"Weapon type {type} doesn't have an effect key");
        }

        public static string HumanReadableName(WeaponType type)
        {
            switch (type)
            {
                case WeaponType.Pistol: return "Pistol";
                case WeaponType.Machete: return "Machete";
                case WeaponType.SubmachineGun: return "SMG";
                case WeaponType.SniperRifle: return "Sniper";
            }

            throw new ArgumentException($"Weapon type {type} doesn't have an effect key");
        }

        public static bool ShouldActivateWeaponAbility(this CardController co, WeaponType type)
        {
            return co.GameController.CanActivateEffect(co.TurnTakerController, EffectKey(type));
        }
    }
}
