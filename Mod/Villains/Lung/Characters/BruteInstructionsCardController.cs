﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Lung
{
    public class BruteInstructionsCardController : VillainCharacterCardController
    {
        /*
         "If there are 10 or more cards in the villain trash, the first time {Lung} is dealt damage each round, reduce the damage by 1.",
        "At the end of the villain turn play the top card of the villain deck then:
        - {Lung} deals X melee damage to all hero targets, where X = 1 + the number of cards in the villain trash / 5, then,
        - If there are 5 or more cards in the villain trash, {Lung} regains 1 HP, then,
        - If there are 15 or more cards in the villain trash, {Lung} regains 1 HP."
          ],
          "flippedGameplay": [
            "{Lung} is immune to damage dealt by Environment cards.",
            "At the end of the villain turn {Lung} deals 6 melee and 2 irreducible fire damage to all hero targets then:
        - Lung regains 3 HP, then,{BR}- Destroy {H - 1} hero ongoing or equipment cards."
        */
        public BruteInstructionsCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(base.TurnTaker.Trash).Condition = () => !base.Card.IsFlipped;
            SpecialStringMaker.ShowIfElseSpecialString(() => HasBeenSetToTrueThisRound("LungReduceDamage"), () => base.Card.Title + " has already reduced damage this round.", () => base.Card.Title + " has not reduced damage this round.").Condition = () => !base.Card.IsFlipped && base.TurnTaker.Trash.NumberOfCards >= 10;
        }

        public override void AddSideTriggers()
        {
            if (Card.IsFlipped)
            {
                AddSideTrigger(AddImmuneToDamageTrigger(dda => dda.DamageSource.Is().Environment() && dda.Target == CharacterCard));
                AddSideTrigger(AddDealDamageAtEndOfTurnTrigger(TurnTaker, TurnTaker.CharacterCard, c => c.Is().Hero().Target(), TargetType.All, 6, DamageType.Melee));
                AddSideTrigger(AddDealDamageAtEndOfTurnTrigger(TurnTaker, TurnTaker.CharacterCard, c => c.Is().Hero().Target(), TargetType.All, 2, DamageType.Fire, isIrreducible: true));
            }
            else
            {
                damageReduceTrigger = AddReduceDamageTrigger(
                    dda => TurnTaker.Trash.NumberOfCards >= 10 && !HasBeenSetToTrueThisRound("LungReduceDamage"),
                    dda => ReduceFirstDamage(dda),
                    c => c == CharacterCard,
                    true
                );

                AddSideTrigger(damageReduceTrigger);
            }

            AddSideTrigger(AddEndOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => DoBruteThings(),
                new TriggerType[] {
                    TriggerType.GainHP,
                    TriggerType.DestroyCard,
                    TriggerType.PlayCard,
                    TriggerType.DealDamage,
                }
            ));
        }

        public IEnumerator DoBruteThings()
        {
            IEnumerator e;
            if (Card.IsFlipped)
            {
                e = GameController.GainHP(TurnTaker.CharacterCard, 3, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }

                e = GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria(c => c.Is().Hero() && (IsOngoing(c) || c.DoKeywordsContain("equipment"))), numberOfCards: Game.H - 1, optional: false, responsibleCard: Card, cardSource: GetCardSource());
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
                e = GameController.PlayTopCard(DecisionMaker, TurnTakerController, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }

                if (Card.IsFlipped) { yield break; }

                /*
                  {Lung} deals X melee damage to all hero targets, where X = 1 + the number of cards in the villain trash / 5, then,
                    - If there are 5 or more cards in the villain trash, {Lung} regains 1 HP, then,
                    - If there are 15 or more cards in the villain trash, {Lung} regains 1 HP."
                */
                int damage = 1 + TurnTaker.Trash.NumberOfCards / 5;
                e = GameController.DealDamage(DecisionMaker, TurnTaker.CharacterCard, c => c.Is().Hero().Target() && c.IsInPlay, damage, DamageType.Melee, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }

                if (Card.IsFlipped) { yield break; }

                if (TurnTaker.Trash.NumberOfCards >= 5)
                {
                    e = GameController.GainHP(TurnTaker.CharacterCard, 1, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(e);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(e);
                    }
                }

                if (Card.IsFlipped) { yield break; }

                if (TurnTaker.Trash.NumberOfCards >= 15)
                {
                    e = GameController.GainHP(TurnTaker.CharacterCard, 1, cardSource: GetCardSource());
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

        public IEnumerator ReduceFirstDamage(DealDamageAction action)
        {
            if (TurnTaker.Trash.NumberOfCards < 10)
            {
                yield break;
            }

            if (HasBeenSetToTrueThisRound("LungReduceDamage"))
            {
                yield break;
            }

            SetCardPropertyToTrueIfRealAction("LungReduceDamage", gameAction: action);

            var e = GameController.ReduceDamage(action, 1, damageReduceTrigger, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        private ITrigger damageReduceTrigger = null;
    }
}
