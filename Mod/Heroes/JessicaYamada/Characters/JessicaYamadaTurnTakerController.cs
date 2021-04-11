using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.JessicaYamada
{
    class JessicaYamadaTurnTakerController : HeroTurnTakerController
    {
        public JessicaYamadaTurnTakerController(TurnTaker tt, GameController gc) : base(tt, gc)
        {
		}

        public override CharacterCardController IncapacitationCardController => CharacterCardControllers.Where(c => c.Card.Identifier == "JessicaYamadaCharacter").First();

		public override bool IsIncapacitated
		{
			get
			{
				return IncapacitationCardController.Card.IsFlipped;
			}
		}

		public override bool IsIncapacitatedOrOutOfGame
		{
			get
			{
				return IsIncapacitated || IncapacitationCardController.Card.IsOutOfGame;
			}
		}
	}
}
