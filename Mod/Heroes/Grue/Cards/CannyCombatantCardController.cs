using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.Grue
{
    public class CannyCombatantCardController : CardController
    {
        public CannyCombatantCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria(c => c.IsOngoing, "Ongoing"));
        }

        public override IEnumerator Play()
        {
            // { GrueCharacter} deals X melee damage to a non - hero target, where X = 2 + the number of Ongoing cards in play
            return GameController.SelectTargetsAndDealDamage(
                HeroTurnTakerController,
                new DamageSource(GameController, CharacterCard),
                c => 2 + OngoingCardCount(),
                DamageType.Melee,
                () => 1,
                optional: false,
                requiredTargets: 1,
                cardSource: GetCardSource()
            );
        }

        private int OngoingCardCount()
        {
            return GameController.FindCardsWhere(c => c.IsOngoing && c.IsInPlay, visibleToCard: GetCardSource(), battleZone: CharacterCard.BattleZone).Count();
        }
    }
}
