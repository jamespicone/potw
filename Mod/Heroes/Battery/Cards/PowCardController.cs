﻿using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Battery
{
    public class PowCardController : BatteryUtilityCardController
    {
        public PowCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            ShowBatteryChargedStatus();
        }

        public override IEnumerator Play()
        {
            // "If {BatteryCharacter} is {Charged}, you may move a non-character target in play back on top of its deck."
            List<MoveCardAction> moves = new List<MoveCardAction>();
            if (IsBatteryCharged())
            {
                List<SelectCardDecision> cardChoices = new List<SelectCardDecision>();
                IEnumerator chooseCoroutine = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.MoveCardOnDeck, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.IsTarget && !c.IsCharacter && base.GameController.IsCardVisibleToCardSource(c, GetCardSource()), "non-character targets", false, singular: "non-character target", plural: "non-character targets"), cardChoices, true, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(chooseCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(chooseCoroutine);
                }
                SelectCardDecision firstChoice = cardChoices.Where((SelectCardDecision scd) => scd.Completed).FirstOrDefault();
                if (firstChoice != null && firstChoice.SelectedCard != null)
                {
                    Card chosen = firstChoice.SelectedCard;
                    IEnumerator moveCoroutine = base.GameController.MoveCard(base.TurnTakerController, chosen, chosen.NativeDeck, storedResults: moves, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(moveCoroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(moveCoroutine);
                    }
                }
            }
            // "If you moved a card this way, {BatteryCharacter} deals 1 non-hero target 3 melee damage. Otherwise, {BatteryCharacter} deals 1 non-hero target 2 melee damage."
            MoveCardAction firstMove = null;
            int meleeAmount = 2;
            if (moves != null)
            {
                Log.Debug("moves != null; updating firstMove");
                firstMove = moves.FirstOrDefault();
            }
            if (firstMove != null && firstMove.CardToMove != null && firstMove.WasCardMoved)
            {
                Log.Debug("firstMove != null, firstMove.CardToMove != null, firstMove.WasCardMoved; updating meleeAmount to 3");
                meleeAmount = 3;
            }
            else
            {
                if (firstMove == null)
                {
                    Log.Debug("firstMove == null; only 2 damage");
                }
                else if (firstMove.CardToMove == null)
                {
                    Log.Debug("firstMove.CardToMove == null; only 2 damage");
                }
                else if (!firstMove.WasCardMoved)
                {
                    Log.Debug("firstMove.WasCardMoved == false; only 2 damage");
                }
            }
            IEnumerator meleeCoroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), meleeAmount, DamageType.Melee, 1, false, 1, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(meleeCoroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(meleeCoroutine);
            }
            yield break;
        }
    }
}
