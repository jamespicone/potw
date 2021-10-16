using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Jp.ParahumansOfTheWormverse.Leviathan
{

    // Whenever a player would draw a card, Leviathan deals that player's character card 1 cold damage. This card is indestructible

    public class TorrentialDownpourCardController : CardController
    {
        public TorrentialDownpourCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            // This card is indestructible
            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            return card == Card;
        }

        public override void AddTriggers()
        {
            // Whenever a player would draw a card...
            AddTrigger<DrawCardAction>(
                dca => dca.IsSuccessful,
                dca => RespondToCardDraw(dca),
                TriggerType.DealDamage,
                TriggerTiming.Before
            );
        }

        public IEnumerator RespondToCardDraw(DrawCardAction dca)
        {
            // ...Leviathan deals that player's character card 1 cold damage
            var stored = new List<Card>();
            var e = FindCharacterCardToTakeDamage(
                dca.HeroTurnTaker,
                stored,
                CharacterCard,
                amount: 1,
                DamageType.Cold
            );
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            var target = stored.FirstOrDefault();
            if (target == null) { yield break; }

            e = DealDamage(CharacterCard, dca.HeroTurnTaker.CharacterCard, 1, DamageType.Cold);
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
