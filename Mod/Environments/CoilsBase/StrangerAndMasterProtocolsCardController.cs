using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.CoilsBase
{
    public class StrangerAndMasterProtocolsCardController : CoilsBaseSelfDestructCardController
    {
        public StrangerAndMasterProtocolsCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            base.AddTriggers();
            // "Hero cards and effects cannot cause other heroes to draw or play cards, use powers, or regain HP."
            // Draw card:
            // CardSource.Card.Owner is the owner of the initiating effect; HeroTurnTaker is the player drawing- if both are heroes but they don't match, prevent the draw
            AddTrigger<DrawCardAction>((DrawCardAction dca) => dca.CardSource != null && dca.CardSource.Card.Owner.IsHero && dca.CardSource.Card.Owner != dca.HeroTurnTaker, (DrawCardAction dca) => CancelAction(dca), TriggerType.CancelAction, TriggerTiming.Before);
            // Play card:
            // DecisionMaker.TurnTaker is the controller of the initiating effect; ResponsibleTurnTaker is USUALLY the player playing- if both are heroes but they don't match, prevent the play
            // Known exception: La Comodora's Chronological Sweetspot says "At the end of your turn, reveal the top card of each hero deck. If the revealed card is a One-Shot, play it. If not, discard it."
            //      This instructs LA COMODORA'S PLAYER to play cards from other hero decks, which shouldn't be prevented by this effect... but is currently implemented with the owner of the card as ResponsibleTurnTaker, so it will be.
            //      A similar problem exists with the Harpy's Swift Summoning, whose power says "Reveal the top card of 1 hero deck. Play a card revealed this way."
            //      Until Handelabra addresses my bug reports for these, the only thing I can do about this is hardcode exceptions for them.
            //      I don't want to do that in case the fix comes in quickly, BUT if you want to do it as a stopgap measure, here's how I would:
            //          AddTrigger<PlayCardAction>((PlayCardAction pca) => pca.CardSource != null && (pca.CardSource.Card.Title != "Chronological Sweetspot" && pca.CardSource.Card.Title != "Swift Summoning") && pca.CardSource.Card.Owner.IsHero && pca.DecisionMaker.IsHero && pca.ResponsibleTurnTaker.IsHero && pca.DecisionMaker.TurnTaker != pca.ResponsibleTurnTaker, (PlayCardAction pca) => CancelAction(pca), TriggerType.CancelAction, TriggerTiming.Before);
            AddTrigger<PlayCardAction>((PlayCardAction pca) => pca.CardSource != null && pca.CardSource.Card.Owner.IsHero && pca.DecisionMaker.IsHero && pca.ResponsibleTurnTaker.IsHero && pca.DecisionMaker.TurnTaker != pca.ResponsibleTurnTaker, (PlayCardAction pca) => CancelAction(pca), TriggerType.CancelAction, TriggerTiming.Before);
            // Use power:
            // CardSource.Card.Owner is the owner of the initiating effect; HeroUsingPower.TurnTaker is the player using the power- if both are heroes but they don't match, prevent the power
            AddTrigger<UsePowerAction>((UsePowerAction upa) => upa.CardSource != null && upa.CardSource.Card.Owner.IsHero && upa.CardSource.Card.Owner != upa.HeroUsingPower.TurnTaker, (UsePowerAction upa) => CancelAction(upa), TriggerType.CancelAction, TriggerTiming.Before);
            // Gain HP:
            // CardSource.Card is the initiating effect; HpGainer is the target gaining HP- if HpGainer is a hero character card and its owner is different from CardSource.Card.Owner, prevent the HP gain
            AddTrigger<GainHPAction>((GainHPAction gha) => gha.CardSource != null && gha.CardSource.Card.Owner.IsHero && gha.HpGainer.IsHeroCharacterCard && gha.CardSource.Card.Owner != gha.HpGainer.Owner, (GainHPAction gha) => CancelAction(gha), TriggerType.CancelAction, TriggerTiming.Before);
            // Search deck:
            // There aren't any existing Sentinels cards that respond to the act of searching a deck, so the engine doesn't give us a SearchDeckAction object we can use to activate this effect. Plus, there are only two official SOTM cards (Micro-Assembler and Freedom Five Wraith) that create effects that search decks other than the one they're from, anyway.
            // I've removed "search their deck" from the card text in the DeckList since I'm not implementing it.
            // HOWEVER. If you want to dig deeper into this, here's what I do know:
            // Hero deck search effects are performed using CardController.SearchForCards()...
            // CardController.SearchForCards() is essentially a wrapper method for GameController.SelectCardsFromLocationAndMoveThem()...
            // GameController.SelectCardsFromLocationAndMoveThem() passes to GameController.SelectLocationAndMoveCard() once it's picked out the card...
            // GameController.SelectLocationAndMoveCard() uses GameController.MoveCard() to move the card once a destination is selected...
            // and from there, it gets complicated, since MoveCard() can create a MoveCardAction or PlayCardAction depending on the card's destination.
            // Plus, there's no guaranteed way of knowing either of those was initiated by a search- the closest I can think of would be "is this card being played/moved from a deck, and if so, is it neither the top card nor the bottom card of that deck?" and that would miss search effects that find their target as the first or last card, and potentially also trigger on non-search effects like "play the card under the top card of your deck", which aren't currently a thing but could certainly be implemented by other mods.
        }
    }
}
