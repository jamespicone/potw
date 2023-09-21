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
    public class PowCardController : BatteryUtilityCardController
    {
        public PowCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.ShowChargedStatus(SpecialStringMaker, CharacterCard);
        }

        public override IEnumerator Play()
        {
            // If {BatteryCharacter} is {Charged}, you may move a non-character target in play back on top of its deck.
            bool didMove = false;
            IEnumerator e;
            if (this.IsCharged(CharacterCard))
            {
                var stored = new List<SelectCardDecision>();
                e = GameController.SelectCardAndStoreResults(
                    HeroTurnTakerController,
                    SelectionType.MoveCardOnDeck,
                    new LinqCardCriteria(c => c.IsInPlayAndHasGameText && c.IsTarget && !c.IsCharacter && !GameController.IsCardIndestructible(c) && GameController.IsCardVisibleToCardSource(c, GetCardSource()), "non-character targets", false, singular: "non-character target", plural: "non-character targets"),
                    stored,
                    optional: true,
                    cardSource: GetCardSource()
                );
                if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
                else { GameController.ExhaustCoroutine(e); }

                var selected = GetSelectedCard(stored);
                if (selected != null)
                {
                    var moves = new List<MoveCardAction>();
                    e = GameController.MoveCard(
                        TurnTakerController,
                        selected,
                        selected.NativeDeck,
                        storedResults: moves,
                        cardSource: GetCardSource()
                    );

                    didMove = DidMoveCard(moves);
                }
            }

            // If you moved a card this way, {BatteryCharacter} deals 1 non-hero target 3 melee damage.
            // Otherwise, {BatteryCharacter} deals 1 non-hero target 2 melee damage."
            int damage = didMove ? 3 : 2;
            e = GameController.SelectTargetsAndDealDamage(
                HeroTurnTakerController,
                new DamageSource(GameController, CharacterCard),
                damage,
                DamageType.Melee,
                numberOfTargets: 1,
                optional: false,
                requiredTargets: 1,
                additionalCriteria: c => c.Is().NonHero().Target().AccordingTo(this),
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }
    }
}
