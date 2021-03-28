using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Armsmaster
{
    public class TargetingComputerCardController : ModuleCardController
    {
        public TargetingComputerCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator DoPrimary()
        {
            var storedResults = new List<SelectTargetDecision>();
            var e = GameController.SelectTargetAndStoreResults(
                HeroTurnTakerController,
                AllCards,
                storedResults,
                additionalCriteria: c => c.IsHero && c.IsTarget,
                selectionType: SelectionType.IncreaseNextDamage,
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

            if (storedResults.Count() <= 0)
            {
                yield break;
            }

            var target = storedResults.First().SelectedCard;

            // Select a hero target. The next time that hero target would deal damage, increase the damage by 1 and make it irreducible
            IncreaseDamageStatusEffect increase = new IncreaseDamageStatusEffect(1)
            {
                NumberOfUses = 1
            };
            increase.SourceCriteria.IsSpecificCard = target;

            MakeDamageIrreducibleStatusEffect irreducible = new MakeDamageIrreducibleStatusEffect
            {
                NumberOfUses = 1
            };
            irreducible.SourceCriteria.IsSpecificCard = target;

            e = GameController.AddStatusEffect(increase, showMessage: true, GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            e = GameController.AddStatusEffect(irreducible, showMessage: true, GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        public IEnumerator HandleSecondaryStatus(PhaseChangeAction unused, OnPhaseChangeStatusEffect sourceEffect)
        {
            var target = sourceEffect.TargetLeavesPlayExpiryCriteria.IsOneOfTheseCards.FirstOrDefault();
            if (target == null) { yield break; }
            if (CharacterCard.IsIncapacitatedOrOutOfGame || !target.IsTarget || !target.IsInPlayAndNotUnderCard) { yield break; }
            if (GameController.IsCardVisibleToCardSource(target, GetCardSource(sourceEffect)))
            {
                var e = GameController.DealDamageToTarget(
                    new DamageSource(GameController, CharacterCard),
                    target,
                    4,
                    DamageType.Projectile
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
            else
            {
                var e = GameController.SendMessageAction(
                    $"{target.Title} is no longer visible",
                    Priority.Medium,
                    GetCardSource(),
                    new[] { target },
                    showCardSource: true
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
        }

        public override IEnumerator DoSecondary()
        {
            // Select a target. At the start of Armsmaster's next turn, he deals that target 4 projectile damage
            var targets = GameController.FindTargetsInPlay();
            var storedTargets = new List<SelectTargetDecision>();
            var e = GameController.SelectTargetAndStoreResults(
                HeroTurnTakerController,
                targets,
                storedTargets,
                damageSource: CharacterCard,
                damageAmount: c => 4,
                damageType: DamageType.Projectile,
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

            Card target = storedTargets.FirstOrDefault()?.SelectedCard;
            if (target == null) { yield break; }

            var effect = new OnPhaseChangeStatusEffect(
                Card,
                nameof(HandleSecondaryStatus),
                $"At the start of {CharacterCard.Title}'s next turn, he deals 4 projectile damage to {target.Title}",
                new[] { TriggerType.DealDamage },
                Card
            );
            effect.UntilEndOfNextTurn(HeroTurnTaker);
            effect.TurnTakerCriteria.IsSpecificTurnTaker = HeroTurnTaker;
            effect.UntilTargetLeavesPlay(target);
            effect.TurnPhaseCriteria.Phase = Phase.Start;
            effect.BeforeOrAfter = BeforeOrAfter.After;
            effect.CanEffectStack = true;
            effect.CardSource = Card;
            effect.NumberOfUses = 1;
            effect.DoesDealDamage = true;

            e = AddStatusEffect(effect);
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
