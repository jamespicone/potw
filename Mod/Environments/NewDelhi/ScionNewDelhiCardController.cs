using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.NewDelhi
{
    public class ScionNewDelhiCardController : CardController
    {
        public ScionNewDelhiCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            // "All cards are indestructible."
            return base.GameController.IsCardVisibleToCardSource(card, GetCardSource());
        }

        public override void AddTriggers()
        {
            // "All targets are immune to damage."
            AddImmuneToDamageTrigger((DealDamageAction dealDamage) => true);
            // "At the start of the environment turn, remove this card from the game (even if it's indestructible)."
            AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => base.GameController.MoveCard(base.TurnTakerController, base.Card, base.TurnTaker.OutOfGame, showMessage: true, responsibleTurnTaker: base.TurnTaker, evenIfIndestructible: true, actionSource: pca, cardSource: GetCardSource()), TriggerType.RemoveFromGame);
            // When indestructibility falls off, check all relevant cards for whether they should be destroyed
            AddAfterLeavesPlayAction((GameAction g) => base.GameController.DestroyAnyCardsThatShouldBeDestroyed(ignoreBattleZone: false, GetCardSource()), TriggerType.DestroyCard);
            AddTrigger((BulkMoveCardsAction bmc) => bmc.CardsToMove.Contains(base.Card) && bmc.Destination.IsOutOfGame, (BulkMoveCardsAction bmc) => base.GameController.DestroyAnyCardsThatShouldBeDestroyed(ignoreBattleZone: true, GetCardSource()), TriggerType.DestroyCard, TriggerTiming.After, ActionDescription.Unspecified, isConditional: false, requireActionSuccess: true, null, outOfPlayTrigger: true);
            AddTrigger((SwitchBattleZoneAction sb) => sb.Origin == base.Card.BattleZone, (SwitchBattleZoneAction sb) => base.GameController.DestroyAnyCardsThatShouldBeDestroyed(ignoreBattleZone: true, GetCardSource()), TriggerType.DestroyCard, TriggerTiming.After, ActionDescription.Unspecified, isConditional: false, requireActionSuccess: true, null, outOfPlayTrigger: false, null, null, ignoreBattleZone: true);
            AddTrigger((MoveCardAction mc) => mc.Origin.BattleZone == base.BattleZone && mc.Destination.BattleZone != base.BattleZone, (MoveCardAction mc) => base.GameController.DestroyAnyCardsThatShouldBeDestroyed(ignoreBattleZone: true, GetCardSource()), TriggerType.DestroyCard, TriggerTiming.After, ActionDescription.Unspecified, isConditional: false, requireActionSuccess: true, null, outOfPlayTrigger: false, null, null, ignoreBattleZone: true);
            base.AddTriggers();
        }
    }
}
