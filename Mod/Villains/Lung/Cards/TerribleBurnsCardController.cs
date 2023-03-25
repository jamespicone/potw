using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

using Jp.SOTMUtilities;

namespace Jp.ParahumansOfTheWormverse.Lung
{
    public class TerribleBurnsCardController : CardController
    {
        public TerribleBurnsCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP(ranking: 1, numberOfTargets: 1);
        }

        public override void AddTriggers()
        {
            // "Whenever a hero is dealt fire damage by {Lung}, destroy 1 hero ongoing or equipment card.",
            AddTrigger<DealDamageAction>(
                dda => dda.DamageType == DamageType.Fire && dda.Target.Is().Hero().Target().Character() && dda.DamageSource.Card == CharacterCard && dda.DidDealDamage,
                dda => RespondToFireDamage(), 
                TriggerType.AddStatusEffectToDamage,
                TriggerTiming.After,
                ActionDescription.DamageTaken
            );

            // "At the end of the villain turn, {Lung} deals the hero target with the highest HP {H - 2} fire damage"
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, CharacterCard, c => c.Is().Hero().Target() && c.IsInPlay, TargetType.HighestHP, Game.H - 2, DamageType.Fire);
        }

        public IEnumerator RespondToFireDamage()
        {
            var e = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria(c => c.Is().Hero() && (IsOngoing(c) || c.DoKeywordsContain("equipment"))), optional: false, responsibleCard: Card, cardSource: GetCardSource());
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
