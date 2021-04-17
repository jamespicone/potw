using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Behemoth
{
    public class JuggernautCardController : BehemothUtilityCardController
    {
        public JuggernautCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // "Reduce damage dealt to {BehemothCharacter} by 1."
            AddReduceDamageTrigger((Card c) => c == base.CharacterCard, 1);
            // "Whenever a hero deals damage to {BehemothCharacter}, move a proximity token from another hero to that hero."
            AddTrigger<DealDamageAction>((DealDamageAction dda) => dda.Target == base.CharacterCard && dda.DamageSource != null && dda.DamageSource.Card.IsHeroCharacterCard && dda.Amount > 0, (DealDamageAction dda) => TakeProximityTokens(dda.DamageSource.Card.Owner, 1), TriggerType.ModifyTokens, TriggerTiming.After);
            base.AddTriggers();
        }

        public override IEnumerator Play()
        {
            // "When this card enters play, change {BehemothCharacter}'s damage type to sonic."
            IEnumerator sonicCoroutine = SetBehemothDamageType(base.Card, DamageType.Sonic);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(sonicCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(sonicCoroutine);
            }
            yield break;
        }
    }
}
