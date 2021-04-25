using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Jp.ParahumansOfTheWormverse.Lung
{
    public class ABBThugsCardController : CardController
    {
        public ABBThugsCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowHeroTargetWithLowestHP(ranking: 1, numberOfTargets: 1);
        }

        public override void AddTriggers()
        {
            // At the end of the villain turn, ABB Thugs deal the hero target with the lowest HP 2 projectile damage
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, c => c.IsHero && c.IsTarget && c.IsInPlay, TargetType.LowestHP, 2, DamageType.Projectile);
        }
    }
}
