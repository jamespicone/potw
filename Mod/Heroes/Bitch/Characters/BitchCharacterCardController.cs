using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.Utility;

namespace Jp.ParahumansOfTheWormverse.Bitch
{
    public class BitchCharacterCardController : HeroCharacterCardController
    {
        public BitchCharacterCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.DoKeywordsContain("dog"), "dog"));
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            // "One target regains 2 HP",
            // "One player may take a card from their trash into their hand",
            // "Reduce damage dealt by environment targets by 2 until the start of your next turn"

            IEnumerator e;
            switch (index)
            {
                case 0:
                    e = GameController.SelectAndGainHP(HeroTurnTakerController, 2, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(e);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(e);
                    }
                    break;

                case 1:
                    e = GameController.SelectHeroToMoveCardFromTrash(HeroTurnTakerController, c => c.HeroTurnTaker.Hand, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(e);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(e);
                    }
                    break;

                case 2:
                    ReduceDamageStatusEffect status = new ReduceDamageStatusEffect(2);
                    status.UntilStartOfNextTurn(TurnTaker);
                    status.SourceCriteria.IsEnvironment = true;
                    e = GameController.AddStatusEffect(status, true, GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(e);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(e);
                    }
                    break;
            }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // Each Dog in play may deal 2 melee damage to a target
            return this.SelectTargetsToDealDamageToTarget(
                HeroTurnTakerController,
                c => c.DoKeywordsContain("dog"),
                c => true,
                2,
                DamageType.Melee
            );
        }
    }
}
