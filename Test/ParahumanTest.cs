using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

namespace Jp.ParahumansOfTheWormverse.UnitTest
{
    public class ParahumanTest : BaseTest
    {
        // Heroes
        protected HeroTurnTakerController alexandria { get { return FindHero("Alexandria"); } }
        protected HeroTurnTakerController armsmaster { get { return FindHero("Armsmaster"); } }
        protected HeroTurnTakerController battery { get { return FindHero("Battery"); } }
        protected HeroTurnTakerController bitch { get { return FindHero("Bitch"); } }
        protected HeroTurnTakerController dauntless { get { return FindHero("Dauntless"); } }
        protected HeroTurnTakerController dragon { get { return FindHero("Dragon"); } }
        protected HeroTurnTakerController grue { get { return FindHero("Grue"); } }
        protected HeroTurnTakerController jessica { get { return FindHero("JessicaYamada"); } }
        protected HeroTurnTakerController labyrinth { get { return FindHero("Labyrinth"); } }
        protected HeroTurnTakerController legend { get { return FindHero("Legend"); } }
        protected HeroTurnTakerController missmilitia { get { return FindHero("MissMilitia"); } }
        protected HeroTurnTakerController skitter { get { return FindHero("Skitter"); } }
        protected HeroTurnTakerController tattletale { get { return FindHero("Tattletale"); } }

        // Villains
        protected TurnTakerController behemoth { get { return FindVillain("Behemoth"); } }
        protected TurnTakerController coil { get { return FindVillain("Coil"); } }
        protected TurnTakerController echidna { get { return FindVillain("Echidna"); } }
        protected TurnTakerController leviathan { get { return FindVillain("Leviathan"); } }
        protected TurnTakerController lung { get { return FindVillain("Lung"); } }
        protected TurnTakerController nine { get { return FindVillain("Slaughterhouse9"); } }
        protected TurnTakerController merchants { get { return FindVillain("TheMerchants"); } }
        protected TurnTakerController simurgh { get { return FindVillain("TheSimurgh"); } }

        // The Nine
        protected Card bonesaw { get { return GetCard("BonesawCharacter"); } }
        protected Card burnscar { get { return GetCard("BurnscarCharacter"); } }
        protected Card cherish { get { return GetCard("CherishCharacter"); } }
        protected Card crawler { get { return GetCard("CrawlerCharacter"); } }
        protected Card jackslash { get { return GetCard("JackSlashCharacter"); } }
        protected Card mannequin { get { return GetCard("MannequinCharacter"); } }
        protected Card shatterbird { get { return GetCard("ShatterbirdCharacter"); } }
        protected Card siberian { get { return GetCard("TheSiberianCharacter"); } }

        // Jessica

        protected Card jessicaCharacter { get { return jessica.CharacterCards.Where(c => c.IsRealCard).First(); } }
        protected Card jessicaInstructions { get { return GetCard("JessicaYamadaInstructionsTarget"); } }

        // Available decks
        protected static string[] ParahumansHeroes = new string[] {
            "Jp.ParahumansOfTheWormverse.Alexandria",
            "Jp.ParahumansOfTheWormverse.Armsmaster",
            "Jp.ParahumansOfTheWormverse.Battery",
            "Jp.ParahumansOfTheWormverse.Bitch",
            "Jp.ParahumansOfTheWormverse.Dauntless",
            "Jp.ParahumansOfTheWormverse.Dragon",
            "Jp.ParahumansOfTheWormverse.Grue",
            "Jp.ParahumansOfTheWormverse.JessicaYamada",
            "Jp.ParahumansOfTheWormverse.Labyrinth",
            "Jp.ParahumansOfTheWormverse.Legend",
            "Jp.ParahumansOfTheWormverse.MissMilitia",
            "Jp.ParahumansOfTheWormverse.Skitter",
            "Jp.ParahumansOfTheWormverse.Tattletale"
        };

        protected static string[] ParahumansVillains = new string[] {
            "Jp.ParahumansOfTheWormverse.Behemoth",
            "Jp.ParahumansOfTheWormverse.Coil",
            "Jp.ParahumansOfTheWormverse.Echidna",
            "Jp.ParahumansOfTheWormverse.Leviathan",
            "Jp.ParahumansOfTheWormverse.Lung",
            "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
            "Jp.ParahumansOfTheWormverse.TheMerchants",
            "Jp.ParahumansOfTheWormverse.TheSimurgh"
        };

        protected static string[] ParahumansEnvironments = new string[] {
            "Jp.ParahumansOfTheWormverse.BrocktonBay",
            "Jp.ParahumansOfTheWormverse.CoilsBase",
            "Jp.ParahumansOfTheWormverse.Kyushu",
            "Jp.ParahumansOfTheWormverse.NewDelhi"
        };

        // Helpers

        // Find all Twisted in play and move them back to their deck
        public void ReturnAllTwisted()
        {
            var twisted = FindCardsWhere(c => c.DoKeywordsContain("twisted"));
            if (twisted.Count() <= 0) { return; }

            MoveCards(echidna, twisted, twisted.First().NativeDeck);
        }

        // Find all Twisted in play and move them out of the game
        public void RemoveAllTwisted()
        {
            var twisted = FindCardsWhere(c => c.DoKeywordsContain("twisted"));
            if (twisted.Count() <= 0) { return; }

            MoveCards(echidna, twisted, twisted.First().Owner.OutOfGame);
        }

        // If the Siberian is in play, puts her under the Nine card.
        public void ReturnSiberian()
        {
            if (siberian.IsInPlayAndNotUnderCard)
            {
                MoveCard(nine, siberian, nine.CharacterCard.UnderLocation, overrideIndestructible: true);
            }
        }

        protected void AssertDamageSource(Card c)
        {
            InstallObserver();
            expectedDamageSource = c;
        }
        protected void AssertDamageType(DamageType type)
        {
            InstallObserver();
            expectedDamageType = type;
        }

        protected void AssertIrreducible()
        {
            InstallObserver();
            expectedIrreducible = true;
        }

        protected void AssertNotIrreducible()
        {
            InstallObserver();
            expectedIrreducible = false;
        }

        protected void IncapacitateCharacter(Card character, Card damageSource)
        {
            SetHitPoints(character, 1);
            DealDamage(damageSource, character, 1, DamageType.Infernal, isIrreducible: true);
        }

        protected void AssertNextRevealReveals(List<Card> cards)
        {
            InstallObserver();
            expectedRevealedCards = cards;
        }

        protected void AssertNextRevealReveals(params Card[] cards)
        {
            AssertNextRevealReveals(cards.ToList());
        }

        // Implementation
        private bool hasInstalledHandler = false;

        private DamageType? expectedDamageType = null;
        private Card expectedDamageSource = null;
        private bool? expectedIrreducible = null;

        private List<Card> expectedRevealedCards = null;

        private IEnumerator ObserveAction(GameAction action)
        {
            if (action is DealDamageAction dda)
            {
                if (expectedDamageType != null)
                {
                    Assert.That(dda.DamageType, Is.EqualTo(expectedDamageType.Value), $"{expectedDamageType} was the expected damage type");
                    expectedDamageType = null;
                }

                if (expectedDamageSource != null)
                {
                    Assert.That(dda.DamageSource.Card, Is.EqualTo(expectedDamageSource), $"{expectedDamageSource.Title} was the expected damage source");
                    expectedDamageSource = null;
                }

                if (expectedIrreducible != null)
                {
                    Assert.That(dda.IsIrreducible, Is.EqualTo(expectedIrreducible.Value), $"{expectedIrreducible.Value} was the expected irreducible status");
                    expectedIrreducible = null;
                }
            }

            if (action is RevealCardsAction ra)
            {
                if (expectedRevealedCards != null)
                {
                    Assert.That(ra.RevealedCards, Is.EquivalentTo(expectedRevealedCards), $"Expected to see {string.Join(", ", expectedRevealedCards.Select(c => c.Title))} revealed but instead saw {string.Join(", ", ra.RevealedCards.Select(c => c.Title))}");
                    expectedRevealedCards = null;
                }
            }

            yield break;
        }

        private void InstallObserver()
        {
            if (! hasInstalledHandler)
            {
                GameController.OnDidPerformAction += ObserveAction;
                hasInstalledHandler = true;
            }
        }
    }
}