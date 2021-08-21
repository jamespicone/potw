using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Model;

namespace Jp.ParahumansOfTheWormverse.Utility
{
    public static class ExtensionMethods
    {
        // Same as IsEnvironmentTarget and IsVillainTarget but checks hero-target status
        // (This may be different to hero-ness because of targetKind decklist property)
        public static bool IsHeroTarget(this Card c)
        {
			if (c.TargetKind == DeckDefinition.DeckKind.Hero) { return true; }

			return c.IsHero && c.IsTarget;
		}
    }
}
