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
    public class BehemothCharacterCardController : BehemothUtilityCharacterCardController
    {
        public const string MovementTrashIdentifier = "MovementTrash";

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
                //Log.Debug("BehemothCharacterCardController.CurrentDamageType(): got index " + currentIndex.Value.ToString() + " from the journal");
                //Log.Debug("BehemothCharacterCardController.CurrentDamageType(): damage type: " + typeOptions[currentIndex.Value].ToString());
                return typeOptions[currentIndex.Value];
            }
            else
            {
                Log.Debug("BehemothCharacterCardController.CurrentDamageType(): Couldn't get the currentIndex from the journal");
            }
            return DamageType.Radiant;
        }

        public override void AddSideTriggers()
        {
            base.AddSideTriggers();
            if(!base.Card.IsFlipped)
            {
                // Herokiller
                // "All damage dealt by {BehemothCharacter} is of {BehemothCharacter}'s current damage type."
                base.AddSideTrigger(AddTrigger<DealDamageAction>((DealDamageAction dda) => dda.DamageSource != null && dda.DamageSource.Card == base.Card, MatchTypeResponse, TriggerType.ChangeDamageType, TriggerTiming.Before));
                // "At the start of the villain turn, if {BehemothCharacter} has less than 30 HP, flip {HeroTacticsCharacter} and {BehemothCharacter}."
                base.AddSideTrigger(AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker && base.Card.HitPoints.Value < 30, FlipBothCharactersResponse, TriggerType.FlipCard));
                // "At the start of the villain turn, play the top card of the Movement deck."
                base.AddSideTrigger(AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, PlayMovementResponse, TriggerType.PlayCard));
                // "At the end of the villain turn, {BehemothCharacter} deals each hero X damage, where X is the number of proximity tokens on that hero."
                base.AddSideTrigger(AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, ProximityBasedDamageResponse, TriggerType.DealDamage));

                if (base.IsGameAdvanced)
                {
                    // "Whenever a hero has 6 proximity tokens, incapacitate that hero."
                    base.AddSideTrigger(AddTrigger<ModifyTokensAction>((ModifyTokensAction mta) => mta.TokenPool.CardWithTokenPool.Owner == base.TurnTaker && mta.TokenPool.Identifier == ProximityPoolIdentifier && mta.TokenPool.CurrentValue >= 6, KillAuraResponse, TriggerType.DestroyCard, TriggerTiming.After));
                }
            }
            else
            {
                // Herokiller, Desperate
                // "All damage dealt by {BehemothCharacter} is of {BehemothCharacter}'s current damage type."
                base.AddSideTrigger(AddTrigger<DealDamageAction>((DealDamageAction dda) => dda.DamageSource != null && dda.DamageSource.Card == base.Card, MatchTypeResponse, TriggerType.ChangeDamageType, TriggerTiming.Before));
                // "Whenever {BehemothCharacter} would be dealt damage of his current damage type, redirect that damage to the hero target with the lowest HP."
                base.AddSideTrigger(AddTrigger((DealDamageAction dda) => dda.Target == base.Card && dda.DamageType == CurrentDamageType(), RedirectToLowestHeroTargetResponse, TriggerType.RedirectDamage, TriggerTiming.Before));
                // "At the start of the villain turn, play the top card of the Movement deck."
                base.AddSideTrigger(AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, PlayMovementResponse, TriggerType.PlayCard));
                // "At the end of the villain turn, {BehemothCharacter} deals each hero X damage, where X is the number of proximity tokens on that hero."
                base.AddSideTrigger(AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, ProximityBasedDamageResponse, TriggerType.DealDamage));
                // "At the end of the villain turn, play the top card of the villain deck."
                base.AddSideTrigger(AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, PlayTheTopCardOfTheVillainDeckResponse, TriggerType.PlayCard));

                if (base.IsGameAdvanced)
                {
                    // "Whenever a hero has 6 proximity tokens, incapacitate that hero."
                    base.AddSideTrigger(AddTrigger<ModifyTokensAction>((ModifyTokensAction mta) => mta.TokenPool.CardWithTokenPool.Owner == base.TurnTaker && mta.TokenPool.Identifier == ProximityPoolIdentifier && mta.TokenPool.CurrentValue >= 6, KillAuraResponse, TriggerType.DestroyCard, TriggerTiming.After));
                }
            }

            // The Movement Deck is a deck, it's not in play
            base.Card.UnderLocation.OverrideIsInPlay = false;

            // If Behemoth is destroyed, the heroes win
            base.AddDefeatedIfDestroyedTriggers();
            base.AddDefeatedIfMovedOutOfGameTriggers();
        }

        public IEnumerator SetDamageType(Card responsibleCard, DamageType newType)
        {
            base.SetCardProperty(LastSetType, Array.IndexOf(typeOptions, newType));
            List<Card> associated = new List<Card>();
            if (responsibleCard != null)
            {
                associated.Add(responsibleCard);
            }
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

        public IEnumerator MatchTypeResponse(DealDamageAction dda)
        {
            // "All damage dealt by {BehemothCharacter} is of {BehemothCharacter}'s current damage type."
            if (dda.DamageType != CurrentDamageType())
            {
                IEnumerator typeCoroutine = base.GameController.ChangeDamageType(dda, CurrentDamageType(), cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(typeCoroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(typeCoroutine);
                }
            }
            yield break;
        }

        public IEnumerator PlayMovementResponse(PhaseChangeAction pca)
        {
            //Log.Debug("BehemothCharacterCardController.PlayMovementResponse activated");
            //Log.Debug("Activated by PhaseChangeAction: " + pca.ToString());
            // If the Movement deck is empty, refill it from the Movement trash and shuffle
            if (!base.Card.UnderLocation.HasCards)
            {
                IEnumerable<Card> movementTrashPile = base.TurnTaker.FindCard(MovementTrashIdentifier, realCardsOnly: false).UnderLocation.Cards;
                IEnumerator shuffleCoroutine = base.GameController.ShuffleCardsIntoLocation(DecisionMaker, movementTrashPile, base.Card.UnderLocation, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(shuffleCoroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(shuffleCoroutine);
                }
            }
            // Play the top card of the Movement deck
            IEnumerator playCoroutine = base.GameController.PlayTopCardOfLocation(base.TurnTakerController, base.Card.UnderLocation, responsibleTurnTaker: base.TurnTaker, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(playCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(playCoroutine);
            }
            yield break;
        }

        public IEnumerator ProximityBasedDamageResponse(PhaseChangeAction pca)
        {
            //Log.Debug("BehemothCharacterCardController.ProximityBasedDamageResponse activated.");
            // "... {BehemothCharacter} deals each hero X damage, where X is the number of proximity tokens on that hero."
            IEnumerable<HeroTurnTakerController> heroControllers = FindActiveHeroTurnTakerControllers();
            //Log.Debug("heroControllers.Count(): " + heroControllers.Count().ToString());
            List<Card> targets = new List<Card>();
            foreach(HeroTurnTakerController player in heroControllers)
            {
                TokenPool playerProximity = ProximityPool(player.TurnTaker);
                Log.Debug(player.Name + "'s proximity pool has " + playerProximity.CurrentValue.ToString() + " tokens.");
                //Log.Debug("Finding hero character card for " + player.Name + "...");
                if (player.HasMultipleCharacterCards)
                {
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
                    string targetNames = (from Card c in charResults select c.Title).ToCommaList(useWordAnd: true);
                    Log.Debug("Adding " + targetNames + " to list of targets");
                    targets.AddRange(charResults);
                }
                else
                {
                    Log.Debug("Adding " + player.CharacterCard.Title + " to list of targets");
                    targets.Add(player.CharacterCard);
                }
            }
            //Log.Debug("Target list complete. Assigning damage...");
            Log.Debug("CurrentDamageType: " + CurrentDamageType().ToString());
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
            // "... flip {HeroTacticsCharacter} and {BehemothCharacter}."
            List<CardController> cardsToFlip = new List<CardController>();
            cardsToFlip.Add(base.GameController.FindCardController(base.TurnTaker.FindCard(HeroTacticsIdentifier)));
            cardsToFlip.Add(this);
            foreach(CardController cc in cardsToFlip)
            {
                IEnumerator flipCoroutine = base.GameController.FlipCard(cc, actionSource: pca, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(flipCoroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(flipCoroutine);
                }
            }
            yield break;
        }

        public IEnumerator KillAuraResponse(ModifyTokensAction mta)
        {
            // "... incapacitate that hero."
            TurnTaker player = mta.TokenPool.CardWithTokenPool.Location.HighestRecursiveLocation.OwnerTurnTaker;
            string message1 = "";
            string message2 = "";
            if (player.IsMultiCharacterTurnTaker)
            {
                message1 = player.NameRespectingVariant + " have " + ProximityPool(player).CurrentValue + " proximity tokens!";
                message2 = base.Card.Title + " has caught " + player.NameRespectingVariant + " within his instant kill aura! " + player.NameRespectingVariant + " are out of the fight!";
            }
            else
            {
                message1 = player.NameRespectingVariant + " has " + ProximityPool(player).CurrentValue + " proximity tokens!";
                message2 = base.Card.Title + " has caught " + player.NameRespectingVariant + " within his instant kill aura! " + player.NameRespectingVariant + " is out of the fight!";
            }
            IEnumerator messageCoroutine1 = base.GameController.SendMessageAction(message1, Priority.High, GetCardSource(), associatedCards: base.GameController.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsHeroCharacterCard && c.Owner == player)), showCardSource: true);
            IEnumerator messageCoroutine2 = base.GameController.SendMessageAction(message2, Priority.High, GetCardSource(), associatedCards: base.GameController.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsHeroCharacterCard && c.Owner == player)), showCardSource: true);
            IEnumerator incapCoroutine = base.GameController.DestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => c.IsHeroCharacterCard && c.Owner == mta.TokenPool.CardWithTokenPool.Location.HighestRecursiveLocation.OwnerTurnTaker && c.Owner.IsHero && !c.IsIncapacitatedOrOutOfGame), selectionType: SelectionType.IncapacitateHero, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(messageCoroutine1);
                yield return GameController.StartCoroutine(messageCoroutine2);
                yield return GameController.StartCoroutine(incapCoroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(messageCoroutine1);
                GameController.ExhaustCoroutine(messageCoroutine2);
                GameController.ExhaustCoroutine(incapCoroutine);
            }
            yield break;
        }
    }
}
