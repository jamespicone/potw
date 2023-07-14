using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;

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
    }
}