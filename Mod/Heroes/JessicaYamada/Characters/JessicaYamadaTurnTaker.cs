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

        public override bool IsIncapacitated => CharacterCard?.IsFlipped ?? false;

        public override bool IsIncapacitatedOrOutOfGame => IsIncapacitated || (CharacterCard?.IsOutOfGame ?? false);

        public override IEnumerator StartGame()
        {
            var e = GameController.BulkMoveCards(this, TurnTaker.OffToTheSide.Cards.Where(c => c.PromoIdentifierOrIdentifier == CharacterIdentifier()), TurnTaker.PlayArea);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }

            e = GameController.BulkMoveCards(this, TurnTaker.OffToTheSide.Cards, TurnTaker.InTheBox);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        private string CharacterIdentifier()
        {
            var id = base.CharacterCard.PromoIdentifierOrIdentifier;
            return id.Replace("Instructions", "Character");
        }
    }
}
