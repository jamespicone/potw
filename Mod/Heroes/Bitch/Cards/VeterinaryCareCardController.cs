using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Bitch
{
    public class VeterinaryCareCardController : CardController
    {
        public VeterinaryCareCardController(Card card, TurnTakerController controller) : base(card, controller)
        { }

        public override IEnumerator Play()
        {
            var cards = GameController.FindCardsWhere(card => card.IsInPlay && card.DoKeywordsContain("dog"), true, GetCardSource());
            foreach (var card in cards)
            {
                if (card.MaximumHitPoints is int hp)
                {
                    var e2 = GameController.SetHP(card, hp, GetCardSource());
                    if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e2); }
                    else { GameController.ExhaustCoroutine(e2); }
                }
            }

            var e = DrawCard(HeroTurnTaker, optional: true);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }

            e = SelectAndPlayCardFromHand(HeroTurnTakerController);
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }
    }
}
