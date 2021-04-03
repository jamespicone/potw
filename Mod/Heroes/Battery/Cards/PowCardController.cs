using Handelabra;
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
                    if (firstChoice.Choices.Count() == 1)
                    {
                        IEnumerator messageCoroutine = base.GameController.SendMessageAction(chosen.Identifier + " is the only non-character target in play.", Priority.Low, GetCardSource(), showCardSource: true);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(messageCoroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(messageCoroutine);
                        }
                    }
                    IEnumerator moveCoroutine = base.GameController.MoveCard(base.TurnTakerController, chosen, chosen.NativeDeck, cardSource: GetCardSource());
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
                firstMove = moves.FirstOrDefault();
            }
            if (firstMove != null && firstMove.CardToMove != null && firstMove.WasCardMoved)
            {
                meleeAmount = 3;
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
