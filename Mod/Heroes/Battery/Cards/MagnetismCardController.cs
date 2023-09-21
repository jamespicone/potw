using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Battery
{
    public class MagnetismCardController : BatteryUtilityCardController
    {
        public MagnetismCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.ShowChargedStatus(SpecialStringMaker, CharacterCard);

            SpecialStringMaker.ShowNumberOfCardsAtLocations(
                () => from httc in GameController.FindHeroTurnTakerControllers() where !httc.IsIncapacitatedOrOutOfGame select httc.TurnTaker.Trash,
                new LinqCardCriteria(c => IsEquipment(c), "equipment")
            );
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // If {BatteryCharacter} is {Charged}, destroy a non-character card Equipment or Device card.
            if (this.IsCharged(CharacterCard))
            {
                var e = GameController.SelectAndDestroyCard(
                    HeroTurnTakerController,
                    new LinqCardCriteria(c => !c.IsCharacter && (c.Is(this).Equipment() || c.Is(this).WithKeyword("device")), "non-character Equipment or Device"),
                    optional: false, 
                    responsibleCard: CharacterCard,
                    cardSource: GetCardSource()
                );
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }

            // If {BatteryCharacter} is {Discharged}, put an Equipment card from a hero trash into play.
            if (! this.IsCharged(CharacterCard))
            {
                // TODO: This should be cards from a hero trash, not hero cards from a trash
                var e = GameController.SelectAndPlayCard(
                    HeroTurnTakerController,
                    c => IsEquipment(c) && c.Location.IsTrash && c.Location.OwnerTurnTaker.Is().Hero().AccordingTo(this),
                    optional: false,
                    isPutIntoPlay: true,
                    cardSource: GetCardSource()
                );
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }
            }
        }
    }
}
