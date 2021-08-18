using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.MissMilitia
{
    public class SubmachineGunCardController : WeaponCardController
    {
        public SubmachineGunCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController, "{smg}")
        {
            ShowWeaponStatusIfActive(MacheteKey);
            ShowWeaponStatusIfActive(SniperRifleKey);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int numTargets = GetPowerNumeral(0, 3);
            int amount = GetPowerNumeral(1, 1);
            
            // "{MissMilitiaCharacter} deals up to 3 non-hero targets 1 projectile damage each."
            var e = GameController.SelectTargetsAndDealDamage(
                HeroTurnTakerController,
                new DamageSource(GameController, CharacterCard),
                amount,
                DamageType.Projectile,
                numTargets,
                false,
                requiredTargets: 0,
                additionalCriteria: (c) => ! c.IsHeroTarget(),
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            // "{machete} You may destroy an Ongoing or environment card."
            if (ActivateWeaponEffectForPower(MacheteKey))
            {
                e = GameController.SelectAndDestroyCard(
                    HeroTurnTakerController,
                    new LinqCardCriteria((c) => c.DoKeywordsContain("ongoing") || c.IsEnvironment, "Ongoing or environment"),
                    optional: true,
                    responsibleCard: Card,
                    cardSource: GetCardSource()
                );
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }
            }

            // "{sniper} You may put a non-hero non-character target in play on top of its deck."
            if (ActivateWeaponEffectForPower(SniperRifleKey))
            {
                var cardChoices = new List<SelectCardDecision>();
                e = GameController.SelectCardAndStoreResults(
                    HeroTurnTakerController,
                    SelectionType.MoveCardOnDeck,
                    new LinqCardCriteria((c) => c.IsInPlayAndHasGameText && ! c.IsHeroTarget() && c.IsTarget && ! c.IsCharacter && GameController.IsCardVisibleToCardSource(c, GetCardSource()), "non-hero non-character targets", false, singular: "non-hero non-character target", plural: "non-hero non-character targets"),
                    cardChoices,
                    optional: true,
                    cardSource: GetCardSource()
                );
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }

                var firstChoice = cardChoices.Where((SelectCardDecision scd) => scd.Completed).FirstOrDefault();
                var chosen = firstChoice?.SelectedCard;
                if (chosen != null)
                {
                    e = GameController.MoveCard(TurnTakerController, chosen, chosen.NativeDeck, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(e);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(e);
                    }
                }
            }
        }
    }
}
