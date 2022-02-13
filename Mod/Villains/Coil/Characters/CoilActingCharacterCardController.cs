using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Coil
{
    public class CoilActingCharacterCardController : SubCoilCharacterCardController
    {
        public CoilActingCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
        }

        protected override void AddCoilTriggers()
        {
            //"If Acting's HP is greater than or equal to Scheming's HP, use {magic} text.",

            //"At the start of the villain turn reveal cards from the villain deck until you reveal a Parahuman or you've revealed 1 ({magic}: {H - 1}) cards. Put any revealed Parahumans into play and shuffle the other cards into the villain deck.",
            AddSideTrigger(AddStartOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => FindParahumans(),
                TriggerType.RevealCard
            ));

            //"At the end of the villain turn Acting deals 2 ({magic}: {H}) energy damage to the hero target with the highest HP",
            AddSideTrigger(AddEndOfTurnTrigger(
                tt => tt == TurnTaker,
                pca => ShootHero(),
                TriggerType.DealDamage
            ));
        }

        private IEnumerator FindParahumans()
        {
            var cardsToReveal = UseExtraText() ? H - 1 : 1;

            IEnumerator e;
            while (cardsToReveal > 0)
            {
                --cardsToReveal;

                var revealed = new List<Card>();
                e = GameController.RevealCards(
                    TurnTakerController,
                    TurnTaker.Deck,
                    1,
                    revealed,
                    revealedCardDisplay: RevealedCardDisplay.ShowRevealedCards,
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

                var revealedCard = revealed.FirstOrDefault();
                if (revealedCard == null || revealedCard.DoKeywordsContain("parahuman"))
                {
                    break;
                }
            }

            var parahumans = TurnTaker.Revealed.Cards.Where(c => c.DoKeywordsContain("parahuman"));
            e = GameController.MoveCards(
                TurnTakerController,
                parahumans,
                TurnTaker.PlayArea,
                isPutIntoPlay: true,
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

            e = CleanupRevealedCards(TurnTaker.Revealed, TurnTaker.Deck, shuffleAfterwards: true);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        private IEnumerator ShootHero()
        {
            var damageAmount = UseExtraText() ? H : 2;
            var e = DealDamageToHighestHP(
                Card,
                1,
                c => c.Is().Hero().Target(),
                c => damageAmount,
                DamageType.Energy
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
}
