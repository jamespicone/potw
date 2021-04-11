using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jp.ParahumansOfTheWormverse.Behemoth
{
    public class BehemothCharacterCardController : BehemothUtilityCharacterCardController
    {
        public BehemothCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            List<string> damageRelevantIdentifiers = new List<string>(){ "BehemothCharacter", "ContinuousCrackle", "Discharge", "Earthquake", "LightningBolt", "Roar", "Wildfire"};
            List<Card> damageRelevantCards = new List<Card>();
            foreach(string id in damageRelevantIdentifiers)
            {
                damageRelevantCards.Add(base.TurnTaker.FindCard(id));
            }
            SpecialStringMaker.ShowSpecialString(() => base.Card.Title + "'s current damage type is " + CurrentDamageType().ToString() + ".", showInEffectsList: () => true, relatedCards: () => damageRelevantCards);
            SpecialStringMaker.ShowHeroTargetWithLowestHP(ranking: 1, numberOfTargets: 1).Condition = () => base.Card.IsFlipped;
        }

        public override bool AllowFastCoroutinesDuringPretend
        { 
            get
            {
                if (!base.Card.IsFlipped)
                {
                    return true;
                }
                else
                {
                    return IsLowestHitPointsUnique((Card c) => c.IsHero);
                }
            }
        }

        public DamageType[] typeOptions = { DamageType.Cold, DamageType.Energy, DamageType.Fire, DamageType.Infernal, DamageType.Lightning, DamageType.Melee, DamageType.Projectile, DamageType.Psychic, DamageType.Radiant, DamageType.Sonic, DamageType.Toxic };
        public const string LastSetType = "LastSetType";

        public DamageType CurrentDamageType()
        {
            int? currentIndex = base.GetCardPropertyJournalEntryInteger(LastSetType);
            if (currentIndex.HasValue)
            {
                return typeOptions[currentIndex.Value];
            }
            return DamageType.Psychic;
        }

        public override void AddSideTriggers()
        {
            base.AddSideTriggers();
            if(!base.Card.IsFlipped)
            {
                // Herokiller
                // "All damage dealt by {BehemothCharacter} is of {BehemothCharacter}'s current damage type."
                AddChangeDamageTypeTrigger((DealDamageAction dda) => dda.DamageSource != null && dda.DamageSource.Card == base.Card, CurrentDamageType());
                // "At the start of the villain turn, if {BehemothCharacter} has less than 30 HP, flip {BehemothCharacter} and {HeroTacticsCharacter}."
                AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker && base.Card.HitPoints.Value < 30, FlipBothCharactersResponse, TriggerType.FlipCard);
                // "At the start of the villain turn, play the top card of the Movement deck."
                AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => base.GameController.PlayTopCardOfLocation(null, base.TurnTaker.FindSubDeck(MovementDeckIdentifier), responsibleTurnTaker: base.TurnTaker, cardSource: GetCardSource()), TriggerType.PlayCard);
                // "At the end of the villain turn, {BehemothCharacter} deals each hero X damage, where X is the number of proximity tokens on that hero."
                AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, ProximityBasedDamageResponse, TriggerType.DealDamage);

                if (base.IsGameAdvanced)
                {
                    // "Whenever a hero has 6 proximity tokens, incapacitate that hero."
                    AddTrigger<ModifyTokensAction>((ModifyTokensAction mta) => mta.TokenPool.CardWithTokenPool.Owner == base.TurnTaker && mta.TokenPool.Identifier == ProximityPoolIdentifier && mta.TokenPool.CurrentValue >= 6, (ModifyTokensAction mta) => base.GameController.DestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => c.IsHeroCharacterCard && c.Owner == mta.TokenPool.CardWithTokenPool.Location.HighestRecursiveLocation.OwnerTurnTaker && c.Owner.IsHero && !c.IsIncapacitatedOrOutOfGame), selectionType: SelectionType.IncapacitateHero, cardSource: GetCardSource()), TriggerType.DestroyCard, TriggerTiming.After);
                }
            }
            else
            {
                // Herokiller, Desperate
                // "All damage dealt by {BehemothCharacter} is of {BehemothCharacter}'s current damage type."
                AddChangeDamageTypeTrigger((DealDamageAction dda) => dda.DamageSource != null && dda.DamageSource.Card == base.Card, CurrentDamageType());
                // "Whenever {BehemothCharacter} would be dealt damage of his current damage type, redirect that damage to the hero target with the lowest HP."
                AddTrigger((DealDamageAction dda) => dda.Target == base.Card && dda.DamageType == CurrentDamageType(), RedirectToLowestHeroTargetResponse, TriggerType.RedirectDamage, TriggerTiming.Before);
                // "At the start of the villain turn, play the top card of the Movement deck."
                AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => base.GameController.PlayTopCardOfLocation(base.TurnTakerController, base.TurnTaker.FindSubDeck(MovementDeckIdentifier), responsibleTurnTaker: base.TurnTaker, cardSource: GetCardSource()), TriggerType.PlayCard);
                // "At the end of the villain turn, {BehemothCharacter} deals each hero X damage, where X is the number of proximity tokens on that hero."
                AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, ProximityBasedDamageResponse, TriggerType.DealDamage);
                // "At the end of the villain turn, play the top card of the villain deck."
                AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, PlayTheTopCardOfTheVillainDeckResponse, TriggerType.PlayCard);

                if (base.IsGameAdvanced)
                {
                    // "Whenever a hero has 6 proximity tokens, incapacitate that hero."
                    AddTrigger<ModifyTokensAction>((ModifyTokensAction mta) => mta.TokenPool.CardWithTokenPool.Owner == base.TurnTaker && mta.TokenPool.Identifier == ProximityPoolIdentifier && mta.TokenPool.CurrentValue >= 6, (ModifyTokensAction mta) => base.GameController.DestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => c.IsHeroCharacterCard && c.Owner == mta.TokenPool.CardWithTokenPool.Location.HighestRecursiveLocation.OwnerTurnTaker && c.Owner.IsHero && !c.IsIncapacitatedOrOutOfGame), selectionType: SelectionType.IncapacitateHero, cardSource: GetCardSource()), TriggerType.DestroyCard, TriggerTiming.After);
                }
            }

            // If Behemoth is destroyed, the heroes win
            base.AddDefeatedIfDestroyedTriggers();
            base.AddDefeatedIfMovedOutOfGameTriggers();
        }

        public IEnumerator SetDamageType(Card responsibleCard, DamageType newType)
        {
            base.SetCardProperty(LastSetType, Array.IndexOf(typeOptions, newType));
            List<Card> associated = new List<Card>();
            associated.Add(responsibleCard);
            string message = base.Card.Title + "'s damage type is now " + CurrentDamageType().ToString() + ".";
            IEnumerator messageCoroutine = base.GameController.SendMessageAction(message, Priority.Medium, GetCardSource(), associatedCards: associated);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(messageCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(messageCoroutine);
            }
            yield break;
        }

        public IEnumerator ProximityBasedDamageResponse(PhaseChangeAction pca)
        {
            // "... {BehemothCharacter} deals each hero X damage, where X is the number of proximity tokens on that hero."
            IEnumerable<HeroTurnTakerController> heroControllers = FindActiveHeroTurnTakerControllers();
            List<Card> targets = new List<Card>();
            foreach(HeroTurnTakerController player in heroControllers)
            {
                if (player.HasMultipleCharacterCards)
                {
                    TokenPool playerProximity = ProximityPool(player.TurnTaker);
                    List<Card> charResults = new List<Card>();
                    IEnumerator findCoroutine = FindCharacterCardToTakeDamage(player.TurnTaker, charResults, base.Card, playerProximity.CurrentValue, CurrentDamageType());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(findCoroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(findCoroutine);
                    }
                    targets.AddRange(charResults);
                }
                else
                {
                    targets.Add(player.CharacterCard);
                }
            }
            IEnumerator damageCoroutine = DealDamage(base.Card, (Card c) => targets.Contains(c), ProximityTokens, CurrentDamageType());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(damageCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(damageCoroutine);
            }
            yield break;
        }

        public IEnumerator RedirectToLowestHeroTargetResponse(DealDamageAction dda)
        {
            // "... redirect that damage to the hero target with the lowest HP."
            List<Card> lowestResults = new List<Card>();
            IEnumerator findCoroutine = base.GameController.FindTargetWithLowestHitPoints(1, (Card c) => c.IsHero && base.GameController.IsCardVisibleToCardSource(c, GetCardSource()), lowestResults, dda, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(findCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(findCoroutine);
            }
            if (lowestResults.Count() > 0)
            {
                Card newTarget = lowestResults.FirstOrDefault();
                IEnumerator redirectCoroutine = base.GameController.RedirectDamage(dda, newTarget, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(redirectCoroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(redirectCoroutine);
                }
            }
            yield break;
        }

        public IEnumerator FlipBothCharactersResponse(PhaseChangeAction pca)
        {
            // "... flip {BehemothCharacter} and {HeroTacticsCharacter}."
            List<CardController> cardsToFlip = new List<CardController>();
            cardsToFlip.Add(this);
            cardsToFlip.Add(base.GameController.FindCardController(base.TurnTaker.FindCard(HeroTacticsIdentifier)));
            IEnumerator flipCoroutine = base.GameController.FlipCards(cardsToFlip, GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(flipCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(flipCoroutine);
            }
            yield break;
        }
    }
}
