using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
using System.Collections;
using System.Linq;

namespace Jp.ParahumansOfTheWormverse.Slaughterhouse9
{
    public class Slaughterhouse9TurnTakerController : TurnTakerController
    {
        public Slaughterhouse9TurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        { }

        public override IEnumerator StartGame()
        {
            var allNine = TurnTaker.GetAllCards().Where(c => c.DoKeywordsContain("nine")).ToList();
            var startInPlay = allNine.TakeRandom(GameController.Game.H, GameController.Game.RNG).ToList();

            foreach (Card c in startInPlay)
            {
                var e = GameController.PlayCard(this, c, isPutIntoPlay: true);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }
                allNine.Remove(c);
            }

            var e2 = GameController.BulkMoveCards(this, allNine, CharacterCard.UnderLocation);
            var e3 = GameController.ShuffleLocation(CharacterCard.UnderLocation);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e2);
                yield return GameController.StartCoroutine(e3);
            }
            else
            {
                GameController.ExhaustCoroutine(e2);
                GameController.ExhaustCoroutine(e3);
            }
        }
    }
}
