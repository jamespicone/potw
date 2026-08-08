using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Jp.ParahumansOfTheWormverse.Tattletale;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Slaughterhouse9
{
    [TestFixture()]
    public class JackSlashTests : Slaughterhouse9TestBase
    {
        [Test()]
        public void TestAttack()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Megalopolis"
            );

            PutMemberInPlay("JackSlashCharacter");

            QuickHPStorage(alexandria.CharacterCard);

            var prisoners = PlayCard("WeDontTakePrisoners");
            DestroyCard(prisoners);

            QuickHPCheck(-2);

            PlayCard("AlexandriasCape");

            prisoners = PlayCard("WeDontTakePrisoners");
            DestroyCard(prisoners);

            QuickHPCheck(-4);
        }

        [Test()]
        public void TestCanBeIncapped()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Megalopolis"
            );

            PutMemberInPlay("JackSlashCharacter");

            DealDamage(jackslash, jackslash, 30, DamageType.Melee);

            AssertFlipped(jackslash);
        }

        [Test()]
        public void TestSpecialPlaysTheTopCardOfTheVillainDeck()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Megalopolis"
            );

            PutMemberInPlay("JackSlashCharacter");

            StackDeck("Spiderbots");

            var legendary = PlayCard("Legendary");
            DestroyCard(legendary);

            AssertIsInPlay("Spiderbots");
        }

        [Test()]
        public void TestAttackOnlyTriggersOncePerTurn()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Megalopolis"
            );

            StartGame();
            PutMemberInPlay("JackSlashCharacter");
            ReturnMembersExcept(jackslash);

            QuickHPStorage(alexandria.CharacterCard);

            var prisoners = PlayCard("WeDontTakePrisoners");
            DestroyCard(prisoners);
            QuickHPCheck(-2);

            prisoners = PlayCard("WeDontTakePrisoners", 1);
            DestroyCard(prisoners);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestFlippedDealsDamageToTheHeroWhoUsedAPower()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Slaughterhouse9",
                "Legacy",
                "Megalopolis"
            );

            StartGame();
            PutMemberInPlay("JackSlashCharacter");
            ReturnMembersExcept(jackslash);

            // A second villain target so flipping Jack doesn't end the game.
            PlayCard("Spiderbots");
            RemoveVillainDeck();

            DealDamage(jackslash, jackslash, 100, DamageType.Melee);
            AssertFlipped(jackslash);

            GoToUsePowerPhase(legacy);

            QuickHPStorage(legacy.CharacterCard);
            UsePower(legacy.CharacterCard);
            QuickHPCheck(-1);
        }
    }
}
