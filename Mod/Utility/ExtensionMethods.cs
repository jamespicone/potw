using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.Utility
{
    // The values are deliberately set up so that the bottom bit is on for the 'non' case.
    // It's not a full bitflags thing.
    public enum CardAlignment
    {
        Hero = 0,
        Nonhero = 1,
        Environment = 2,
        Nonenvironment = 3,
        Villain = 4,
        Nonvillain = 5
    };

    public enum CardTarget
    {
        Either,
        Target,
        Nontarget
    };

    public static class ExtensionMethods
    {

        public static bool HasAlignmentCharacter(this CardController controller, Card c, CardAlignment alignment, CardTarget target = CardTarget.Either)
        {
            return controller.HasAlignment(c, alignment, target) && c.IsCharacter;
        }

        public static CardAlignmentHelper Alignment(this Card c, CardController controller = null)
        {
            return new CardAlignmentHelper(c, controller);
        }

        public static CardAlignmentHelper Alignment(this TurnTaker t, CardController controller = null)
        {
            return new CardAlignmentHelper(t, controller);
        }

        public static CardAlignmentHelper Alignment(this TurnTakerController t, CardController controller = null)
        {
            return new CardAlignmentHelper(t.TurnTaker, controller);
        }

        public static CardAlignmentHelper Alignment(this DamageSource source, CardController controller = null)
        {
            if (source.IsCard)
            {
                return source.Card.Alignment(controller);
            }
            else
            {
                return source.TurnTaker.Alignment(controller);
            }
        }

        public static bool HasAlignment(this CardController controller, Card c, CardAlignment alignment, CardTarget target = CardTarget.Either)
        {
            if (c == null) { return false; }

            // CardAlignment enum is deliberately set up so that the bottom bit is 'non-'.
            var baseAlignment = (CardAlignment)(((int)alignment) & ~1);

            if (controller == null && baseAlignment == CardAlignment.Villain)
            {
                throw new InvalidOperationException("HasAlignment called with null controller and villain alignment");
            }

            bool hasBaseAlignment = false;
            switch(baseAlignment)
            {
                case CardAlignment.Hero:
                    if (target == CardTarget.Target) { hasBaseAlignment = c.TargetKind == DeckDefinition.DeckKind.Hero || c.IsHero; }
                    else { hasBaseAlignment = c.IsHero; }
                    break;

                case CardAlignment.Villain:
                    if (target == CardTarget.Target) { hasBaseAlignment = controller.GameController.AskCardControllersIfIsVillainTarget(c, controller.GetCardSource()); }
                    else { hasBaseAlignment = controller.GameController.AskCardControllersIfIsVillain(c, controller.GetCardSource()); }
                    break;

                case CardAlignment.Environment:
                    if (target == CardTarget.Target) { hasBaseAlignment = c.IsEnvironmentTarget; }
                    else { hasBaseAlignment = c.IsEnvironment; }
                    break;

                default:
                    break;
            }

            var isNonCase = (((int)alignment) & 1) > 0;
            if (isNonCase)
            {
                hasBaseAlignment = !hasBaseAlignment;
            }

            switch(target)
            {
                case CardTarget.Target: hasBaseAlignment = hasBaseAlignment && c.IsTarget; break;
                case CardTarget.Nontarget: hasBaseAlignment = hasBaseAlignment && ! c.IsTarget; break;
                case CardTarget.Either:
                default:
                    break;
            }

            return hasBaseAlignment;
        }

        public static bool HasAlignment(this CardController controller, TurnTaker t, CardAlignment alignment)
        {
            if (t == null) { return false; }

            // CardAlignment enum is deliberately set up so that the bottom bit is 'non-'.
            var baseAlignment = (CardAlignment)(((int)alignment) & ~1);

            bool hasBaseAlignment = false;
            switch (baseAlignment)
            {
                case CardAlignment.Hero:
                    hasBaseAlignment = t.IsHero;
                    break;

                case CardAlignment.Villain:
                    hasBaseAlignment = controller.IsVillain(t);
                    break;

                case CardAlignment.Environment:
                    hasBaseAlignment = t.IsEnvironment;
                    break;

                default:
                    break;
            }

            var isNonCase = (((int)alignment) & 1) > 0;
            if (isNonCase)
            {
                hasBaseAlignment = !hasBaseAlignment;
            }

            return hasBaseAlignment;
        }

        public static IEnumerator SelectTargetsToDealDamageToTarget(
            this CardController c,
            HeroTurnTakerController decisionMaker,
            Func<Card, bool> damageDealerCriteria,
            Func<Card, bool> targetCriteria,
            int amount,
            DamageType damageType
        )
        {
            return c.SelectTargetsToDealDamageToTarget(
                decisionMaker,
                damageDealerCriteria,
                damageDealer => c.GameController.SelectTargetsAndDealDamage(
                    decisionMaker,
                    new DamageSource(c.GameController, damageDealer),
                    amount,
                    damageType,
                    1,
                    optional: false,
                    requiredTargets: 0,
                    additionalCriteria: card => targetCriteria(card),
                    cardSource: c.GetCardSource()
                )
            );
        }

        public static IEnumerator SelectTargetsToDealDamageToTarget(
            this CardController c,
            HeroTurnTakerController decisionMaker,
            Func<Card, bool> damageDealerCriteria,
            Func<Card, IEnumerator> damageFunc
        )
        {
            var alreadySelected = new List<Card>();
            bool autoDecided = false;
            IEnumerator e;

            while (true)
            {
                var selectable = c.GameController.FindCardsWhere(card => card.IsInPlay && damageDealerCriteria(card), true, c.GetCardSource()).Except(alreadySelected);
                if (selectable.Count() <= 0) { break; }

                Card selectedDamageDealer;

                if (autoDecided)
                {
                    selectedDamageDealer = selectable.First();
                }
                else
                {
                    var storedResults = new List<SelectCardDecision>();
                    e = c.GameController.SelectCardAndStoreResults(
                        decisionMaker,
                        SelectionType.CardToDealDamage,
                        new LinqCardCriteria(card => selectable.Contains(card)),
                        storedResults,
                        optional: false,
                        allowAutoDecide: true,
                        cardSource: c.GetCardSource()
                    );
                    if (c.UseUnityCoroutines)
                    {
                        yield return c.GameController.StartCoroutine(e);
                    }
                    else
                    {
                        c.GameController.ExhaustCoroutine(e);
                    }

                    selectedDamageDealer = storedResults.FirstOrDefault()?.SelectedCard;
                    if (selectedDamageDealer == null) { break; }

                    autoDecided = storedResults.FirstOrDefault().AutoDecided;
                }

                alreadySelected.Add(selectedDamageDealer);

                e = damageFunc(selectedDamageDealer);

                if (c.UseUnityCoroutines)
                {
                    yield return c.GameController.StartCoroutine(e);
                }
                else
                {
                    c.GameController.ExhaustCoroutine(e);
                }
            }

            yield break;
        }
    }
}
