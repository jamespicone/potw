using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Armsmaster
{
    public abstract class ModuleCardController : CardController
    {
        public static string PrimarySecondaryKey = "IsPrimary";

        public ModuleCardController(Card card, TurnTakerController controller) : base(card, controller)
        {
            SpecialStringMaker.ShowIfElseSpecialString(() => IsPrimaryModule(base.GameController, base.Card), () => base.Card.Title + " is attached as Primary.", () => base.Card.Title + " is attached as Secondary.").Condition = () => base.Card.IsInPlayAndHasGameText;
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            var e = SelectCardThisCardWillMoveNextTo(
                new LinqCardCriteria(
                    c =>
                        c.DoKeywordsContain("halberd") &&
                        (
                            c == CharacterCard || 
                            c.GetAllNextToCards(false).All(c2 => ! IsPrimaryModule(GameController, c2)) ||
                            c.GetAllNextToCards(false).All(c2 => ! IsSecondaryModule(GameController, c2))
                        ),
                    "halberd",
                    useCardsSuffix: true
                ),
                storedResults,
                isPutIntoPlay,
                decisionSources
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

        public override IEnumerator Play()
        {
            var ownerCard = Card.Location.OwnerCard;
            if (ownerCard == null) { yield break; }
            
            bool primaryAllowed = true;
            bool secondaryAllowed = true;

            if (ownerCard != CharacterCard)
            {
                foreach (var c in ownerCard.GetAllNextToCards(false))
                {
                    if (c == Card) { continue; }
                    if (IsPrimaryModule(GameController, c))
                    {
                        primaryAllowed = false;
                    }

                    if (IsSecondaryModule(GameController, c))
                    {
                        secondaryAllowed = false;
                    }
                }
            }

            if (primaryAllowed && secondaryAllowed)
            {
                var primarySecondary = new List<SelectWordDecision>();
                var e = GameController.SelectWord(
                    HeroTurnTakerController,
                    new string[] { "Primary", "Secondary" },
                    SelectionType.Custom,
                    primarySecondary,
                    optional: false,
                    new Card[] { Card },
                    GetCardSource()
                );

                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e);
                }
                else
                {
                    GameController.ExhaustCoroutine(e);
                }

                if (primarySecondary.FirstOrDefault().Index == 0)
                {
                    SetCardProperty(PrimarySecondaryKey, true);
                }
                else
                {
                    SetCardProperty(PrimarySecondaryKey, false);
                }
            }
            else
            {
                SetCardProperty(PrimarySecondaryKey, primaryAllowed);
            }
        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {
            return new CustomDecisionText(
                "Attach module as Primary or Secondary?",
                HeroTurnTaker.NameRespectingVariant + " is selecting module type",
                "Vote for attaching the module as Primary or Secondary",
                "Attach module as Primary or Secondary?"
            );
        }

        public abstract IEnumerator DoPrimary();

        public abstract IEnumerator DoSecondary();

        public override IEnumerator ActivateAbility(string abilityKey)
        {
            IEnumerator e = null;
            if (abilityKey == "primary")
            {
                e = DoPrimary();
            }

            if (abilityKey == "secondary")
            {
                e = DoSecondary();
            }

            if (e == null)
            {
                yield break;
            }

            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e);
            }
            else
            {
                GameController.ExhaustCoroutine(e);
            }
        }

        public bool? IsPrimary()
        {
            return GetCardPropertyJournalEntryBoolean(PrimarySecondaryKey);
        }

        public static bool IsPrimaryModule(GameController gc, Card card)
        {
            var controller = gc.FindCardController(card) as ModuleCardController;
            if (controller == null) { return false; }

            return controller.IsPrimary() == true;
        }

        public static bool IsSecondaryModule(GameController gc, Card card)
        {
            var controller = gc.FindCardController(card) as ModuleCardController;
            if (controller == null) { return false; }

            return controller.IsPrimary() == false;
        }
    }
}
