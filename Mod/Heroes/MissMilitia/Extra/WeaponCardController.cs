using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jp.ParahumansOfTheWormverse.MissMilitia;

namespace Jp.ParahumansOfTheWormverse.MissMilitia
{
    public abstract class WeaponCardController : MissMilitiaUtilityCardController
    {
        protected WeaponType Type
        {
            get;
            private set;
        }

        public WeaponCardController(Card card, TurnTakerController turnTakerController, WeaponType type)
            : base(card, turnTakerController)
        {
            Type = type;
        }

        public sealed override IEnumerator UsePower(int index = 0)
        {
            var effect = new ActivateEffectStatusEffect(TurnTaker, Card, MissMilitiaExtensions.EffectKey(Type));
            effect.UntilEndOfNextTurn(TurnTaker);

            bool activateAllEffects = false;
            var controller = FindCardController(CharacterCard) as MissMilitiaProtectorateCaptainCharacterCardController;
            if (controller != null)
            {
                activateAllEffects = controller.ConsumeActivateAllWeaponEffects();
            }            

            var e = AddStatusEffect(effect);
            var e2 = DoWeaponEffect(activateAllEffects);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
                yield return GameController.StartCoroutine(e2);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
                GameController.ExhaustCoroutine(e2);
            }
        }

        protected abstract IEnumerator DoWeaponEffect(bool activateAllEffects);
    }
}
