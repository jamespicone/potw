using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Random;
using System.IO;
using Jp.ParahumansOfTheWormverse.UnitTest;

namespace Handelabra.Sentinels.UnitTest
{
    // Subclass this to make your own random test cases - See MyRandomTest.cs for an example!
    [TestFixture]
    public class RandomGameTest : ParahumanTest
    {
        private RandomSource rng = new MersenneTwister();
        private Random seededRng;

        protected IEnumerable<string> PreferredCardsToPlay = null;

        protected GameController SetupRandomGameController(bool reasonable, IEnumerable<string> availableHeroes = null, IEnumerable<string> availableVillains = null, IEnumerable<string> availableEnvironments = null,
            string overrideEnvironment = null, List<string> useHeroes = null, bool randomizeUseHeroes = true, Dictionary<string, string> overrideVariants = null, string overrideVillain = null)
        {
            string environment = overrideEnvironment;
            string villain = overrideVillain;
            var heroes = new List<string>();
            var promoIdentifiers = new Dictionary<string, string>();

            if (availableHeroes == null)
            {
                availableHeroes = DeckDefinition.AvailableHeroes;
            }

            if (availableVillains == null)
            {
                availableVillains = DeckDefinition.AvailableVillains;
            }

            if (availableEnvironments == null)
            {
                availableEnvironments = DeckDefinition.AvailableEnvironments;
            }

            if (overrideVariants == null)
            {
                overrideVariants = new Dictionary<string, string>();
            }

            if (useHeroes != null)
                useHeroes = useHeroes.Distinct().ToList();

            // Choose a villain
            var villainName = "";
            if (villain != null)
            {
                var villainInfo = GetRandomVariant(villain, promoIdentifiers);
                villainName = villainInfo.Values.FirstOrDefault();
            }
            else
            {
                var villainInfo = GetRandomVillain(availableVillains, promoIdentifiers);
                villain = villainInfo.Keys.FirstOrDefault();
                villainName = villainInfo.Values.FirstOrDefault();
            }

            if (overrideVariants.ContainsKey(villain))
            {
                var villainInfo = GetSpecificVariant(villain, overrideVariants[villain], promoIdentifiers);
                villainName = villainInfo.Values.FirstOrDefault();
            }

            Log.Debug(villainName + " threatens the Multiverse!");

            // Choose an environment
            if (environment == null)
            {
                environment = GetRandomEnvironment(availableEnvironments);
            }

            Log.Debug(environment + " is the location of the conflict.");

            // Choose heroes
            var heroesLeft = availableHeroes.ToList();
            int numHeroes = Math.Max(GetRandomNumber(3, 6), useHeroes?.Count ?? 0);
            while (heroes.Count < numHeroes)
            {
                string hero = "";
                string heroName = "";
                if (useHeroes != null && useHeroes.Count() > 0)
                {
                    var heroInfo = randomizeUseHeroes ? GetRandomHero(useHeroes, promoIdentifiers) : GetRandomVariant(useHeroes.First(), promoIdentifiers);
                    hero = heroInfo.FirstOrDefault().Key;
                    heroName = heroInfo.FirstOrDefault().Value;
                    useHeroes.Remove(hero);
                }
                else
                {
                    var heroInfo = GetRandomHero(heroesLeft, promoIdentifiers);
                    hero = heroInfo.FirstOrDefault().Key;
                    heroName = heroInfo.FirstOrDefault().Value;
                }

                if (overrideVariants.ContainsKey(hero))
                {
                    var heroInfo = GetSpecificVariant(hero, overrideVariants[hero], promoIdentifiers);
                    heroName = heroInfo.Values.FirstOrDefault();
                }

                Log.Debug(heroName + " joins the team!");
                heroes.Add(hero);
                heroesLeft.Remove(hero);
            }

            bool advanced = (GetRandomNumber(2) == 1);

            List<string> identifiers = MakeGameIdentifiers(villain, heroes, environment);

            var seed = GetRandomNumber();
            var gc = SetupGameController(identifiers, advanced, promoIdentifiers, randomSeed: seed);
            this.seededRng = gc.Game.RNG;

            gc.OnMakeDecisions -= MakeDecisions;
            if (reasonable)
            {
                gc.OnMakeDecisions += MakeSomewhatReasonableDecisions;
            }
            else
            {
                gc.OnMakeDecisions += MakeRandomDecisions;
            }

            return gc;
        }

        private List<string> MakeGameIdentifiers(string villain, List<string> heroes, string environment)
        {
            var result = new List<string>();
            result.Add(villain);
            result.AddRange(heroes);
            result.Add(environment);

            return result;
        }

        private Dictionary<string, string> GetRandomHero(List<string> availableHeroes, Dictionary<string, string> promoIdentifiers)
        {
            int index = GetRandomNumber(availableHeroes.Count);
            string hero = availableHeroes.ElementAt(index);

            return GetRandomVariant(hero, promoIdentifiers);
        }

        private string GetRandomEnvironment(IEnumerable<string> availableEnvironments)
        {
            int environmentIndex = GetRandomNumber(availableEnvironments.Count());
            return availableEnvironments.ElementAt(environmentIndex);
        }

        private Dictionary<string, string> GetRandomVillain(IEnumerable<string> availableVillains, Dictionary<string, string> promoIdentifiers)
        {
            int index = GetRandomNumber(availableVillains.Count());
            string villain = availableVillains.ElementAt(index);

            return GetRandomVariant(villain, promoIdentifiers);
        }

        private Dictionary<string, string> GetRandomVariant(string identifier, Dictionary<string, string> promoIdentifiers)
        {
            var definition = DeckDefinitionCache.GetDeckDefinition(identifier);
            var promosToCheck = new List<CardDefinition>();
            promosToCheck.AddRange(definition.PromoCardDefinitions.Where((CardDefinition cd) => cd.Identifier.Contains(definition.Identifier)));
            promosToCheck.AddRange(ModHelper.GetAllPromoDefinitions().Where((CardDefinition cd) => cd.Identifier.Contains(definition.Identifier)));

            var name = definition.Name;

            // If there is a promo, maybe choose it
            int promoIndex = GetRandomNumber(1 + promosToCheck.Count());

            if (promoIndex > 0)
            {
                var promo = promosToCheck.ElementAt(promoIndex - 1);
                name = promo.PromoTitle;
                string ns = definition.Namespace != promo.Namespace ? $"{promo.Namespace}." : "";

                if (identifier != "TheSentinels")
                {
                    promoIdentifiers[identifier] = ns + promo.PromoIdentifier;
                }
                else
                {
                    promoIdentifiers["TheSentinelsInstructions"] = ns + promo.PromoIdentifier;
                    promoIdentifiers["DrMedicoCharacter"] = ns + promo.AssociatedPromoIdentifiers.ElementAt(0);
                    promoIdentifiers["MainstayCharacter"] = ns + promo.AssociatedPromoIdentifiers.ElementAt(1);
                    promoIdentifiers["TheIdealistCharacter"] = ns + promo.AssociatedPromoIdentifiers.ElementAt(2);
                    promoIdentifiers["WritheCharacter"] = ns + promo.AssociatedPromoIdentifiers.ElementAt(3);
                }
            }

            return new Dictionary<string, string> { { identifier, name } };
        }

        private Dictionary<string, string> GetSpecificVariant(string identifier, string promoIdentifier, Dictionary<string, string> promoIdentifiers)
        {
            var definition = DeckDefinitionCache.GetDeckDefinition(identifier);
            var promosToCheck = new List<CardDefinition>();
            promosToCheck.AddRange(definition.PromoCardDefinitions.Where((CardDefinition cd) => cd.Identifier.Contains(definition.Identifier)));
            promosToCheck.AddRange(ModHelper.GetAllPromoDefinitions().Where((CardDefinition cd) => cd.Identifier.Contains(definition.Identifier)));

            var name = definition.Name;

            if (definition.AllInitialCardIdentifiers.Contains(promoIdentifier))
            {
                promoIdentifiers.Remove(identifier);
                return new Dictionary<string, string> { { identifier, name } };
            }

            var promo = promosToCheck.FirstOrDefault((CardDefinition cd) => cd.PromoIdentifier == promoIdentifier);

            if (promo == null)
            {
                // Could probably do better error handling here, like defaulting back to the randomly-chosen variant
                // Since the tester specified a variant, or something went wrong, should let them know to fix
                Assert.Fail($"ERROR: Cannot find variant {promoIdentifier} for {identifier}.");
            }

            name = promo.PromoTitle;
            string ns = definition.Namespace != promo.Namespace ? $"{promo.Namespace}." : "";

            if (identifier != "TheSentinels")
            {
                promoIdentifiers[identifier] = ns + promo.PromoIdentifier;
            }
            else
            {
                promoIdentifiers["TheSentinelsInstructions"] = ns + promo.PromoIdentifier;
                promoIdentifiers["DrMedicoCharacter"] = ns + promo.AssociatedPromoIdentifiers.ElementAt(0);
                promoIdentifiers["MainstayCharacter"] = ns + promo.AssociatedPromoIdentifiers.ElementAt(1);
                promoIdentifiers["TheIdealistCharacter"] = ns + promo.AssociatedPromoIdentifiers.ElementAt(2);
                promoIdentifiers["WritheCharacter"] = ns + promo.AssociatedPromoIdentifiers.ElementAt(3);
            }

            return new Dictionary<string, string> { { identifier, name } };
        }

        protected void RunParticularGame(IEnumerable<string> turnTakers, bool advanced, IDictionary<string, string> promos, int seed, bool reasonable)
        {
            SetupGameController(turnTakers, advanced, promos, seed);
            this.GameController.OnMakeDecisions -= MakeDecisions;
            if (reasonable)
            {
                this.GameController.OnMakeDecisions += MakeSomewhatReasonableDecisions;
            }
            else
            {
                this.GameController.OnMakeDecisions += MakeRandomDecisions;
            }

            this.seededRng = this.GameController.Game.RNG;
            RunGame(this.GameController);
        }

        protected void RunGame(GameController gameController)
        {
            PrintGameSummary();

            RunCoroutine(gameController.StartGame());

            int roundLimit = 100;

            while (!gameController.IsGameOver && gameController.Game.Round <= roundLimit)
            {
                Log.Debug("\n");
                RunCoroutine(gameController.EnterNextTurnPhase());

                var state = gameController.Game.ToStateString();
                Assert.That(state, Is.Not.Null);

                // Prevent infinite loops and let us know what happened.
                int sanityCheck = 10;

                if (gameController.ActiveTurnPhase.IsPlayCard)
                {
                    while (!gameController.IsGameOver && gameController.CanPerformPhaseAction(gameController.ActiveTurnPhase))
                    {
                        PlayCard(gameController.ActiveTurnTakerController);

                        Log.Debug("PLAY CARD SANITY: " + sanityCheck);
                        sanityCheck -= 1;
                        Assert.That(sanityCheck, Is.GreaterThan(0), "Sanity check failed in " + gameController.ActiveTurnPhase);
                    }
                }
                else if (gameController.ActiveTurnPhase.IsUsePower)
                {
                    HeroTurnTakerController hero = gameController.ActiveTurnTakerController.ToHero();
                    while (!gameController.IsGameOver && gameController.CanPerformPhaseAction(gameController.ActiveTurnPhase))
                    {
                        RunCoroutine(gameController.SelectAndUsePower(hero));

                        Log.Debug("POWER SANITY: " + sanityCheck);
                        sanityCheck -= 1;
                        Assert.That(sanityCheck, Is.GreaterThan(0), "Sanity check failed in " + gameController.ActiveTurnPhase);
                    }
                }
                else if (gameController.ActiveTurnPhase.IsDrawCard)
                {
                    while (!gameController.IsGameOver && gameController.CanPerformPhaseAction(gameController.ActiveTurnPhase))
                    {
                        RunCoroutine(gameController.DrawCard(gameController.ActiveTurnTaker.ToHero()));

                        Log.Debug("DRAW CARD SANITY: " + sanityCheck);
                        sanityCheck -= 1;
                        Assert.That(sanityCheck, Is.GreaterThan(0), "Sanity check failed in " + gameController.ActiveTurnPhase);
                    }
                }
                else if (gameController.ActiveTurnPhase.IsUseIncapacitatedAbility)
                {
                    while (!gameController.IsGameOver && gameController.CanPerformPhaseAction(gameController.ActiveTurnPhase))
                    {
                        RunCoroutine(gameController.SelectAndUseIncapacitatedAbility(gameController.ActiveTurnTakerController.ToHero()));

                        Log.Debug("INCAPACITATED SANITY: " + sanityCheck);
                        sanityCheck -= 1;
                        Assert.That(sanityCheck, Is.GreaterThan(0), "Sanity check failed in " + gameController.ActiveTurnPhase);
                    }
                }
            }

            if (gameController.IsGameOver)
            {
                Log.Debug("Game over!");
                Log.Debug(gameController.Game.ToString());
            }
        }

        protected void PlayCard(TurnTakerController taker)
        {
            // If hero, player decides
            if (taker is HeroTurnTakerController)
            {
                RunCoroutine(taker.GameController.SelectAndPlayCard(taker.ToHero(), card => card.Location == taker.ToHero().HeroTurnTaker.Hand, false));
            }
            else
            {
                Log.Debug($"Top card of {taker.TurnTaker.Name} is {taker.TurnTaker.Deck.TopCard?.Identifier}");

                // Otherwise play the top card of the deck.
                RunCoroutine(taker.GameController.PlayTopCard(null, taker));
            }
        }

        protected IEnumerator MakeRandomDecisions(IDecision decision)
        {
            // Make random decisions!
            Log.Debug($"Making decision {decision.ToStringForMultiplayerDebugging()}");
            if (decision.IsOptional)
            {
                // 5% of the time skip optional decisions
                if (GetRandomDecision(5))
                {
                    Log.Debug($"Skipping decision");
                    decision.Skip();
                    yield break;
                }
            }


            if (decision is SelectCardDecision)
            {
                var selectCardDecision = (SelectCardDecision)decision;
                var choices = selectCardDecision.Choices;
                if (selectCardDecision.SelectionType == SelectionType.PlayCard)
                    choices = choices.Where(c => GameController.CanPlayCard(FindCardController(c)) == CanPlayCardResult.CanPlay).ToList();

                if (choices.Count() > 0)
                {
                    selectCardDecision.SelectedCard = choices.ElementAtOrDefault(GetRandomNumber(choices.Count()));
                    Log.Debug($"Selected card {selectCardDecision.SelectedCard.Identifier}");
                }
                else
                {
                    selectCardDecision.Skip();
                    Log.Debug($"Skipping selecting card");
                }
            }
            else if (decision is YesNoDecision)
            {
                var yesNo = decision as YesNoDecision;
                yesNo.Answer = GetRandomNumber(1) == 1;
                Log.Debug($"Selecting answer {yesNo.Answer}");
            }
            else if (decision is SelectDamageTypeDecision)
            {
                var damage = decision as SelectDamageTypeDecision;
                damage.SelectedDamageType = damage.Choices.ElementAtOrDefault(GetRandomNumber(damage.Choices.Count()));
                Log.Debug($"Selected damage type {damage.SelectedDamageType}");
            }
            else if (decision is MoveCardDecision)
            {
                var moveCard = decision as MoveCardDecision;
                moveCard.Destination = moveCard.PossibleDestinations.ElementAtOrDefault(GetRandomNumber(moveCard.PossibleDestinations.Count()));
                Log.Debug($"Selected destination {moveCard.Destination}");
            }
            else if (decision is UsePowerDecision)
            {
                var power = decision as UsePowerDecision;
                power.SelectedPower = power.Choices.ElementAtOrDefault(GetRandomNumber(power.Choices.Count()));
                Log.Debug($"Selected power {power.SelectedPower}");
            }
            else if (decision is UseIncapacitatedAbilityDecision)
            {
                var ability = decision as UseIncapacitatedAbilityDecision;
                ability.SelectedAbility = ability.Choices.ElementAtOrDefault(GetRandomNumber(ability.Choices.Count()));
                Log.Debug($"Selected ability {ability.SelectedAbility}");
            }
            else if (decision is SelectTurnTakerDecision)
            {
                var turn = decision as SelectTurnTakerDecision;
                turn.SelectedTurnTaker = turn.Choices.ElementAtOrDefault(GetRandomNumber(turn.Choices.Count()));
                Log.Debug($"Selected turn taker {turn.SelectedTurnTaker.Name}");
            }
            else if (decision is SelectCardsDecision)
            {
                var cards = decision as SelectCardsDecision;
                cards.ReadyForNext = true;
                Log.Debug("Ready for next");
            }
            else if (decision is SelectFunctionDecision)
            {
                var function = decision as SelectFunctionDecision;
                function.SelectedFunction = function.Choices.ElementAtOrDefault(GetRandomNumber(function.Choices.Count()));
                Log.Debug($"Selected function {function.SelectedFunction}");
            }
            else if (decision is SelectNumberDecision)
            {
                var number = decision as SelectNumberDecision;
                number.SelectedNumber = number.Choices.ElementAtOrDefault(GetRandomNumber(number.Choices.Count()));
                Log.Debug($"Selected number {number.SelectedNumber}");
            }
            else if (decision is SelectLocationDecision)
            {
                var selectLocation = decision as SelectLocationDecision;
                selectLocation.SelectedLocation = selectLocation.Choices.ElementAtOrDefault(GetRandomNumber(selectLocation.Choices.Count()));
                Log.Debug($"Selected location {selectLocation.SelectedLocation}");
            }
            else if (decision is ActivateAbilityDecision)
            {
                var activate = decision as ActivateAbilityDecision;
                activate.SelectedAbility = activate.Choices.ElementAtOrDefault(GetRandomNumber(activate.Choices.Count()));
                Log.Debug($"Selected ability {activate.SelectedAbility.AbilityKey} {activate.SelectedAbility.Index} on {activate.CardSource.Card.Identifier}");
            }
            else if (decision is SelectWordDecision)
            {
                var word = decision as SelectWordDecision;
                word.SelectedWord = word.Choices.ElementAtOrDefault(GetRandomNumber(word.Choices.Count()));
                Log.Debug($"Selected word {word.SelectedWord}");
            }
            else if (decision is SelectFromBoxDecision)
            {
                var box = decision as SelectFromBoxDecision;
                var heroes = DeckDefinition.AvailableHeroes;
                var selectedTurnTaker = heroes.ElementAtOrDefault(GetRandomNumber(heroes.Count()));
                var promos = DeckDefinitionCache.GetDeckDefinition(selectedTurnTaker).PromoCardDefinitions.Select(cd => cd.PromoIdentifier);
                var selectedPromo = promos.ElementAtOrDefault(GetRandomNumber(promos.Count()));
                box.SelectedIdentifier = selectedPromo;
                box.SelectedTurnTakerIdentifier = selectedTurnTaker;
                Log.Debug($"Selected from box {box.SelectedIdentifier} {box.SelectedTurnTakerIdentifier}");
            }
            else if (decision is SelectTurnPhaseDecision)
            {
                var selectPhase = decision as SelectTurnPhaseDecision;
                selectPhase.SelectedPhase = selectPhase.Choices.ElementAtOrDefault(GetRandomNumber(selectPhase.Choices.Count()));
                Log.Debug($"Selected phase {selectPhase.SelectedPhase}");
            }
            else
            {
                Assert.Fail("Unhandled decision: " + decision);
            }

            yield return null;
        }

        protected Card ChooseBestTarget(IEnumerable<Card> targets)
        {
            Card result = null;

            // Prefer the villain target with the lowest HP if available
            result = targets.Where(c => c.IsVillain).OrderBy(c => c.HitPoints).FirstOrDefault();

            if (result == null)
            {
                // Try an environment target with the lowest HP
                result = targets.Where(c => c.IsEnvironmentTarget).OrderBy(c => c.HitPoints).FirstOrDefault();
            }

            if (result == null)
            {
                // Go for the hero with the highest HP
                result = targets.Where(c => c.IsHero).OrderBy(c => c.HitPoints).LastOrDefault();
            }

            if (result == null)
            {
                Log.Debug("ChooseBestTarget had nothing to choose from!");
            }
            else
            {
                Log.Debug("ChooseBestTarget from <" + targets.Select(c => c.Title).ToRecursiveString() + "> choosing " + result.Title);
            }

            return result;
        }

        protected Card ChooseDestroyCard(IEnumerable<Card> cards)
        {
            Card result = null;

            // Prefer a villain card
            result = cards.FirstOrDefault(c => c.IsVillain);

            if (result == null)
            {
                // Try an environment card
                result = cards.FirstOrDefault(c => c.IsEnvironment);
            }

            if (result == null)
            {
                // Go for a hero
                result = cards.FirstOrDefault(c => c.IsHero);
            }

            if (result == null)
            {
                Log.Debug("ChooseDestroyCard had nothing to choose from!");
            }
            else
            {
                Log.Debug("ChooseDestroyCard from <" + cards.Select(c => c.Title).ToRecursiveString() + "> choosing " + result.Title);
            }

            return result;
        }

        protected IEnumerator MakeSomewhatReasonableDecisions(IDecision decision)
        {
            // Make random decisions!
            Log.Debug($"Making decision {decision.ToStringForMultiplayerDebugging()}");
            if (decision is SelectCardDecision)
            {
                var selectCardDecision = (SelectCardDecision)decision;

                if (selectCardDecision.SelectionType == SelectionType.RedirectDamage || selectCardDecision.SelectionType == SelectionType.DealDamage)
                {
                    selectCardDecision.SelectedCard = ChooseBestTarget(selectCardDecision.Choices);

                    if (selectCardDecision.SelectedCard == null)
                    {
                        // Nothing good to choose from, try skipping
                        if (selectCardDecision.IsOptional)
                        {
                            selectCardDecision.Skip();
                        }
                        else
                        {
                            // Just pick something
                            selectCardDecision.SelectedCard = selectCardDecision.Choices.ElementAtOrDefault(GetRandomNumber(selectCardDecision.Choices.Count()));
                        }
                    }
                }
                else if (selectCardDecision.SelectionType == SelectionType.DestroyCard)
                {
                    selectCardDecision.SelectedCard = ChooseDestroyCard(selectCardDecision.Choices);

                    if (selectCardDecision.SelectedCard == null)
                    {
                        // Nothing good to choose from, try skipping
                        if (selectCardDecision.IsOptional)
                        {
                            selectCardDecision.Skip();
                        }
                        else
                        {
                            // Just pick something
                            selectCardDecision.SelectedCard = selectCardDecision.Choices.ElementAtOrDefault(GetRandomNumber(selectCardDecision.Choices.Count()));
                        }
                    }
                }
                else if (selectCardDecision.SelectionType == SelectionType.PlayCard)
                {
                    if (PreferredCardsToPlay != null)
                    {
                        // Try to play a card we prefer
                        foreach (var identifier in PreferredCardsToPlay)
                        {
                            var card = selectCardDecision.Choices.FirstOrDefault(c => c.Identifier == identifier);
                            if (card != null && GameController.CanPlayCard(FindCardController(card)) == CanPlayCardResult.CanPlay)
                            {
                                Log.Debug("Playing preferred card: " + identifier);
                                selectCardDecision.SelectedCard = card;
                                break;
                            }
                        }
                    }

                    if (selectCardDecision.SelectedCard == null)
                    {
                        // Pick a card that can be played
                        var actualChoices = selectCardDecision.Choices.Where(c => GameController.CanPlayCard(FindCardController(c)) == CanPlayCardResult.CanPlay);
                        if (actualChoices.Count() > 0)
                        {
                            selectCardDecision.SelectedCard = actualChoices.ElementAtOrDefault(GetRandomNumber(actualChoices.Count()));
                            Log.Debug($"Choosing to play {selectCardDecision.SelectedCard.Identifier}");
                        }
                        else
                        {
                            // Just skip
                            selectCardDecision.Skip();
                        }
                    }
                }
                else
                {
                    // Pick a random one
                    selectCardDecision.SelectedCard = selectCardDecision.Choices.ElementAtOrDefault(GetRandomNumber(selectCardDecision.Choices.Count()));
                }

                if (selectCardDecision.Skipped)
                {
                    Log.Debug($"Skipped selecting card");
                }
                else
                {
                    Log.Debug($"Selected card {selectCardDecision.SelectedCard}");
                }
            }
            else if (decision is YesNoDecision)
            {
                var yesNo = decision as YesNoDecision;
                yesNo.Answer = GetRandomNumber(1) == 1;
                Log.Debug($"Selected answer {yesNo.Answer}");
            }
            else if (decision is SelectDamageTypeDecision)
            {
                var damage = decision as SelectDamageTypeDecision;
                damage.SelectedDamageType = damage.Choices.ElementAtOrDefault(GetRandomNumber(damage.Choices.Count()));
                Log.Debug($"Selected damage type {damage.SelectedDamageType}");
            }
            else if (decision is MoveCardDecision)
            {
                var moveCard = decision as MoveCardDecision;
                moveCard.Destination = moveCard.PossibleDestinations.ElementAtOrDefault(GetRandomNumber(moveCard.PossibleDestinations.Count()));
                Log.Debug($"Selected destination {moveCard.Destination}");
            }
            else if (decision is UsePowerDecision)
            {
                var power = decision as UsePowerDecision;
                power.SelectedPower = power.Choices.ElementAtOrDefault(GetRandomNumber(power.Choices.Count()));
                Log.Debug($"Selected power {power.SelectedPower}");
            }
            else if (decision is UseIncapacitatedAbilityDecision)
            {
                var ability = decision as UseIncapacitatedAbilityDecision;
                ability.SelectedAbility = ability.Choices.ElementAtOrDefault(GetRandomNumber(ability.Choices.Count()));
                Log.Debug($"Selected ability {ability.SelectedAbility}");
            }
            else if (decision is SelectTurnTakerDecision)
            {
                var turn = decision as SelectTurnTakerDecision;
                turn.SelectedTurnTaker = turn.Choices.ElementAtOrDefault(GetRandomNumber(turn.Choices.Count()));
                Log.Debug($"Selected turntaker {turn.SelectedTurnTaker}");
            }
            else if (decision is SelectCardsDecision)
            {
                var cards = decision as SelectCardsDecision;
                cards.ReadyForNext = true;
                Log.Debug("Ready for next");
            }
            else if (decision is SelectFunctionDecision)
            {
                var function = decision as SelectFunctionDecision;
                function.SelectedFunction = function.Choices.ElementAtOrDefault(GetRandomNumber(function.Choices.Count()));
                Log.Debug($"Selected function {function.SelectedFunction}");
            }
            else if (decision is SelectNumberDecision)
            {
                var number = decision as SelectNumberDecision;
                number.SelectedNumber = number.Choices.ElementAtOrDefault(GetRandomNumber(number.Choices.Count()));
                Log.Debug($"Selected number {number.SelectedNumber}");
            }
            else if (decision is SelectLocationDecision)
            {
                var selectLocation = decision as SelectLocationDecision;
                selectLocation.SelectedLocation = selectLocation.Choices.ElementAtOrDefault(GetRandomNumber(selectLocation.Choices.Count()));
                Log.Debug($"Selected location {selectLocation.SelectedLocation}");
            }
            else if (decision is ActivateAbilityDecision)
            {
                var activateAbility = decision as ActivateAbilityDecision;
                activateAbility.SelectedAbility = activateAbility.Choices.ElementAtOrDefault(GetRandomNumber(activateAbility.Choices.Count()));
                Log.Debug($"Activated ability {activateAbility.SelectedAbility.AbilityKey} {activateAbility.SelectedAbility.Index} on {activateAbility.CardSource.Card.Identifier}");
            }
            else if (decision is SelectWordDecision)
            {
                var selectWord = decision as SelectWordDecision;
                selectWord.SelectedWord = selectWord.Choices.ElementAtOrDefault(GetRandomNumber(selectWord.Choices.Count()));
                Log.Debug($"Selected word {selectWord.SelectedWord}");
            }
            else if (decision is SelectFromBoxDecision)
            {
                var box = decision as SelectFromBoxDecision;
                var heroes = box.Choices;
                var selectedPair = heroes.ElementAtOrDefault(GetRandomNumber(heroes.Count()));
                box.SelectedIdentifier = selectedPair.Value;
                box.SelectedTurnTakerIdentifier = selectedPair.Key;
                Log.Debug($"Selected from box {box.SelectedIdentifier} {box.SelectedTurnTakerIdentifier}");
            }
            else if (decision is SelectTurnPhaseDecision)
            {
                var selectPhase = decision as SelectTurnPhaseDecision;
                selectPhase.SelectedPhase = selectPhase.Choices.ElementAtOrDefault(GetRandomNumber(selectPhase.Choices.Count()));
                Log.Debug($"Selected phase {selectPhase.SelectedPhase}");
            }
            else
            {
                Assert.Fail("Unhandled decision: " + decision);
            }

            yield return null;
        }

        protected int GetRandomNumber(int min, int max)
        {
            if (seededRng != null)
            {
                return seededRng.Next(min, max);
            }
            else
            {
                return rng.Next(min, max);
            }
        }

        protected int GetRandomNumber(int max)
        {
            return GetRandomNumber(0, max);
        }

        protected int GetRandomNumber()
        {
            if (seededRng != null)
            {
                return seededRng.Next();
            }
            else
            {
                return rng.Next();
            }
        }

        protected bool GetRandomDecision(double percentage)
        {
            var number = GetRandomNumber(100);
            return percentage > number;
        }

        // Example test implementations follow
/*
        [Test]
        public void TestRandomGameToCompletion()
        {
            //for (int i = 0; i < 200; i++)
            {
                GameController gameController = SetupRandomGameController(false);
                RunGame(gameController);
            }
        }

        [Test]
        public void TestSomewhatReasonableGameToCompletion()
        {
            GameController gameController = SetupRandomGameController(true);
            RunGame(gameController);
        }

        [Test]
        public void TestSkyScraper()
        {
            //for (int i = 0; i < 10; i++)
            {
                GameController gameController = SetupRandomGameController(false, useHeroes: new List<string> { "SkyScraper" });
                RunGame(gameController);
            }
        }

        [Test]
        public void TestTachyon_SupersonicResponse()
        {
            //for (int i = 0; i < 1000; i++)
            {
                SetupRandomGameController(true, useHeroes: new List<string> { "Tachyon" });
                PreferredCardsToPlay = new string[] { "SupersonicResponse", "FleetOfFoot", "HUDGoggles", "PushingTheLimits" };
                RunGame(this.GameController);
            }
        }

        [Test]
        public void TestCelestialTribunal()
        {
            //for (int i = 0; i < 10; i++)
            {
                GameController gameController = SetupRandomGameController(true, overrideEnvironment:"TheCelestialTribunal");
                RunGame(gameController);
            }
        }

        [Test]
        public void TestRaVersusTheEnnead()
        {
            GameController gameController = SetupGameController(new string[] { "TheEnnead", "Ra", "Legacy", "TheWraith", "TombOfAnubis" });
            gameController.OnMakeDecisions -= MakeDecisions;
            gameController.OnMakeDecisions += MakeSomewhatReasonableDecisions;

            bool expectNemesis = false;
            bool nemesisApplied = false;
            gameController.OnWillPerformAction += action =>
            {
                // If it is a deal damage action between the Ennead and Ra, make sure there is some nemesis damage going on.
                if (action is DealDamageAction)
                {
                    var dd = action as DealDamageAction;
                    if (IsRaVersusTheEnnead(dd))
                    {
                        // We expect to see some nemesis damage before the end.
                        expectNemesis = true;
                    }
                }
                else if (action is IncreaseDamageAction)
                {
                    var increase = action as IncreaseDamageAction;
                    if (increase.IsNemesisEffect)
                    {
                        nemesisApplied = true;
                    }
                }

                return DoNothing();
            };

            gameController.OnDidPerformAction += action =>
            {
                // If we expected nemesis, assert that it was applied.
                if (action is DealDamageAction && expectNemesis)
                {
                    var dd = action as DealDamageAction;
                    if (IsRaVersusTheEnnead(dd) && dd.DidDealDamage)
                    {
                        Assert.IsTrue(nemesisApplied, "Damage was dealt from " + dd.DamageSource.TitleOrName + " and " + dd.Target.Title + ", but nemesis increase was not applied.");
                    }

                    expectNemesis = false;
                    nemesisApplied = false;
                }

                return DoNothing();
            };

            RunGame(gameController);
        }

        private bool IsRaVersusTheEnnead(DealDamageAction dd)
        {
            return dd.DamageSource.IsCard && (dd.DamageSource.Card.Identifier == "RaCharacter" && dd.Target.Owner.Identifier == "TheEnnead")
                || (dd.DamageSource.Card.Owner.Identifier == "TheEnnead" && dd.Target.Identifier == "RaCharacter");
        }
*/
    }
}
