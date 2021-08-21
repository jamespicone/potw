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
    public class MissMilitiaUtilityCharacterCardController : HeroCharacterCardController
    {
        public MissMilitiaUtilityCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public LinqCardCriteria WeaponCard()
        {
            return new LinqCardCriteria((Card c) => c.DoKeywordsContain("weapon"), "weapon");
        }
    }
}
