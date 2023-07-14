using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.SOTMUtilities;
using System.Security.Cryptography;

namespace Jp.ParahumansOfTheWormverse.Labyrinth
{
    public class BeautifulGardenCardController : ShapingCardController
    {
        public BeautifulGardenCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override void AddShapingTriggers()
        {
            // The first time an Environment card would deal damage to a hero target other than Labyrinth each turn prevent that damage.
            AddPreventDamageTrigger(
                dda => dda.DamageSource.Is().Environment().Card() &&
                    dda.Target.Is().Hero().Target().AccordingTo(this) &&
                    dda.Target != CharacterCard &&
                    !HasBeenSetToTrueThisTurn("PreventedDamage"),
                SetProperty,
                followUpTriggerTypes: new TriggerType[]
                {
                    TriggerType.Hidden
                },
                isPreventEffect: true
            );

            // Environment cards cannot destroy hero cards.
            AddTrigger<DestroyCardAction>(
                dca => (
                    (dca.ResponsibleCard != null && dca.ResponsibleCard.Is().Environment()) ||
                    (dca.ResponsibleCard == null && dca.CardSource?.Card != null && dca.CardSource.Card.Is().Environment())
                ) && dca.CardToDestroy.Is().Hero().AccordingTo(this),
                dca => CancelAction(dca),
                TriggerType.CancelAction,
                TriggerTiming.Before
            );
        }

        private IEnumerator SetProperty(DealDamageAction dda)
        {
            SetCardPropertyToTrueIfRealAction("PreventedDamage", gameAction: dda);
            yield break;
        }
    }
}
