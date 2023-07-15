using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.JessicaYamada
{
    public class JessicaYamadaTurnTakerController : HeroTurnTakerController
    {
        public JessicaYamadaTurnTakerController(TurnTaker turntaker, GameController controller) : base(turntaker, controller) {}

        public override Card CharacterCard { get {
            var ret = TurnTaker.FindCard(CharacterIdentifier());

            if (ret == null)
            {
                ret = base.CharacterCard;
            }

            return ret;
        } }

        public override CharacterCardController IncapacitationCardController => FindCardController(CharacterCard) as CharacterCardController;

        public override bool IsIncapacitated
        {
            get
            {
                var ret = CharacterCard?.IsFlipped ?? false;
                return ret;
            }
        }

        public override bool IsIncapacitatedOrOutOfGame
        {
            get
            {
                var ret = IsIncapacitated || (CharacterCard?.IsOutOfGame ?? false);
                return ret;
            }
        }

        public override IEnumerator StartGame()
        {
            var characterCard = TurnTaker.OffToTheSide.Cards.Where(c => c.PromoIdentifierOrIdentifier == CharacterIdentifier()).FirstOrDefault();
            var e = GameController.MoveCard(this, characterCard, TurnTaker.PlayArea);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            // Banish other variants to the box.
            e = GameController.MoveCards(this, TurnTaker.OffToTheSide.Cards, TurnTaker.InTheBox);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            TurnTaker.SetCharacterCard(CharacterCard);
        }

        private string CharacterIdentifier()
        {
            var id = base.CharacterCard.PromoIdentifierOrIdentifier;
            return id.Replace("Instructions", "Character");
        }
    }
}
