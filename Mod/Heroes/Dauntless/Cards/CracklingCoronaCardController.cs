using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Dauntless
{
    public class CracklingCoronaCardController : CardController
    {
        public CracklingCoronaCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            // {DauntlessCharacter} deals 1 energy damage to all villain targets"
            var e = DealDamage(
                CharacterCard,
                c => c.IsVillainTarget,
                1,
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
