using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;
using System.Runtime.InteropServices;
using System;

namespace Jp.ParahumansOfTheWormverse.Labyrinth
{
    public class CallInTheCrewCardController : CardController
    {
        // When you use a power on this card put a token on it. Powers with tokens on them cannot be used.

        private const string BaseTokenPoolName = "PowerPool";
        private const int PowerCount = 5;
        private string[] PowerText = new string[] {
            "Put a non-indestructible noncharacter target on top of its deck.",
            "{LabyrinthCharacter} deals 3 sonic damage to all non-hero targets.",
            "Select a damage type. Until the start of your next turn whenever damage of that type would be dealt reduce it by 1.",
            "{LabyrinthCharacter} deals a target 4 fire damage.",
            "All players draw a card."
        };
        private readonly Func<IEnumerator>[] PowerFunc;

        public CallInTheCrewCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            PowerFunc = new Func<IEnumerator>[] {
                Power0,
                Power1,
                Power2,
                Power3,
                Power4
            };

            AddAsPowerContributor();

            for (int i = 0; i < PowerCount; ++i)
            {
                ShowTokenPool(i);
            }
        }
        
        private void ShowTokenPool(int index)
        {
            var pool = Card.FindTokenPool(BaseTokenPoolName + index);
            if (pool == null) return;

            SpecialStringMaker.ShowTokenPool(pool, this);
        }

        public override IEnumerable<Power> AskIfContributesPowersToCardController(CardController cardController)
        {
            if (cardController != this) { return null; }

            var ret = new List<Power>();

            for (int i = 0; i < PowerCount; ++i)
            {
                if (HasPowerBeenUsed(i)) continue;

                ret.Add(new Power(
                    cardController.HeroTurnTakerController,
                    cardController,
                    PowerText[i],
                    PowerFunc[i],
                    i,
                    null,
                    GetCardSource()
                ));
            }

            return ret;
        }

        public override void AddTriggers()
        {
            AddAfterLeavesPlayAction(ResetUsedPowers); ;
        }

        private bool HasPowerBeenUsed(int powerIndex)
        {
            var pool = Card.FindTokenPool(BaseTokenPoolName + powerIndex);
            if (pool == null) return false;

            return pool.CurrentValue > 0;
        }

        private IEnumerator SetPowerUsed(int powerIndex)
        {
            var pool = Card.FindTokenPool(BaseTokenPoolName + powerIndex);
            if (pool == null) yield break;

            var e = GameController.AddTokensToPool(pool, 1, GetCardSource());
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }

        public IEnumerator ResetUsedPowers()
        {
            foreach (var pool in Card.TokenPools)
            {
                pool.SetToInitialValue();
            }
            yield break;
        }

        private IEnumerator Power0()
        {
            var e = SetPowerUsed(0);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            // "Put a non-indestructible noncharacter target on top of its deck.",
            var storedTarget = new List<SelectCardDecision>();
            e = GameController.SelectCardAndStoreResults(
                HeroTurnTakerController,
                SelectionType.MoveCardOnDeck,
                new LinqCardCriteria(
                    (c) => ! c.IsOneShot &&
                        c.IsInPlay &&
                        c.Is().Noncharacter().Target() &&
                        ! GameController.IsCardIndestructible(c) &&
                        GameController.IsCardVisibleToCardSource(c, GetCardSource()),
                    "non-indestructible non-character target in play", useCardsSuffix: false),
                storedTarget,
                optional: false,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            var selectedTarget = GetSelectedCard(storedTarget);
            if (selectedTarget == null) yield break;

            e = GameController.MoveCard(
                TurnTakerController,
                selectedTarget,
                GetNativeDeck(selectedTarget),
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }

        private IEnumerator Power1()
        {
            var e = SetPowerUsed(1);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            // "{LabyrinthCharacter} deals 3 sonic damage to all non-hero targets.",
            e = DealDamage(
                CharacterCard,
                c => c.Is().NonHero().Target().AccordingTo(this),
                GetPowerNumeral(0, 3),
                DamageType.Sonic
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }

        private IEnumerator Power2()
        {
            var e = SetPowerUsed(2);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            // "Select a damage type. Until the start of your next turn whenever damage of that type would be dealt reduce it by 1.",
            var storedResults = new List<SelectDamageTypeDecision>();
            e = GameController.SelectDamageType(
                HeroTurnTakerController,
                storedResults,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            var type = GetSelectedDamageType(storedResults);
            if (type == null) { yield break; }

            var effect = new ReduceDamageStatusEffect(GetPowerNumeral(0, 1));
            effect.DamageTypeCriteria.AddType(type.Value);
            effect.UntilStartOfNextTurn(TurnTaker);

            e = AddStatusEffect(effect);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }

        private IEnumerator Power3()
        {
            var e = SetPowerUsed(3);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            // "{LabyrinthCharacter} deals a target 4 fire damage.",
            e = GameController.SelectTargetsAndDealDamage(
                HeroTurnTakerController,
                new DamageSource(GameController, CharacterCard),
                GetPowerNumeral(0, 4), DamageType.Fire,
                numberOfTargets: 1,
                optional: false,
                requiredTargets: 1,
                cardSource: GetCardSource()
            );
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }

        private IEnumerator Power4()
        {
            var e = SetPowerUsed(4);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            // "All players draw a card."
            e = EachPlayerDrawsACard(htt => true);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }
    }
}
