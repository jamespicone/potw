using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Slaughterhouse9
{
    [TestFixture()]
    public class Slaughterhouse9Tests : Slaughterhouse9TestBase
    {
        [Test()]
        public void TestModWorks()
        {
            SetupNineGame();

            Assert.That(nine, Is.Not.Null);
        }

        [Test()]
        public void TestSetupDeploysHMembersAndPutsTheRestUnderTheNine()
        {
            SetupNineGame();
            StartGame();

            // H = 3 members deployed, the other 5 shuffled under the character card.
            Assert.That(MembersInPlay.Count(), Is.EqualTo(3));
            Assert.That(nine.CharacterCard.UnderLocation.Cards.Count(), Is.EqualTo(5));
            Assert.That(nine.TurnTaker.Deck.Cards.Any(c => c.DoKeywordsContain("nine")), Is.False);
        }

        [Test()]
        public void TestMemberFlipsInsteadOfBeingDestroyed()
        {
            SetupNineGame();

            var member = PutMemberInPlay("BonesawCharacter");

            DestroyCard(member, haka.CharacterCard);

            AssertFlipped(member);
            AssertIsInPlay(member);
        }

        [Test()]
        public void TestHeroesWinWhenNoVillainTargetsAreInPlay()
        {
            SetupNineGame();
            StartGame();

            // The Siberian makes other members immune to damage; put her back
            // under the Nine so damage can flip whoever was deployed.
            ReturnSiberian();

            var deployed = MembersInPlay.ToList();
            foreach (var member in deployed.Take(deployed.Count - 1))
            {
                DealDamage(haka.CharacterCard, member, 60, DamageType.Melee);
                AssertNotGameOver();
            }

            DealDamage(haka.CharacterCard, deployed.Last(), 60, DamageType.Melee);

            AssertGameOver(EndingResult.VillainDestroyedVictory);
        }

        [Test()]
        public void TestAdvancedDeploysAMemberAtTheEndOfTheVillainTurn()
        {
            SetupGameController(
                new string[] { "Jp.ParahumansOfTheWormverse.Slaughterhouse9", "Legacy", "Bunker", "Haka", "Megalopolis" },
                advanced: true);
            StartGame();

            // Empty the villain deck so the turn's card play adds no noise.
            MoveCards(nine, nine.TurnTaker.Deck.Cards.ToList(), nine.TurnTaker.OutOfGame);

            var expected = nine.CharacterCard.UnderLocation.TopCard;
            var membersBefore = MembersInPlay.Count();

            GoToEndOfTurn(nine);

            Assert.That(MembersInPlay.Count(), Is.EqualTo(membersBefore + 1));
            AssertIsInPlay(expected);
        }
    }
}
