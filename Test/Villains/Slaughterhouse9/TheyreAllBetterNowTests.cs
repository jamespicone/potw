using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Slaughterhouse9
{
    [TestFixture()]
    public class TheyreAllBetterNowTests : Slaughterhouse9TestBase
    {
        [Test()]
        public void TestRevivesMostRecentlyDefeatedNine()
        {
            // The revive finds the defeated member via the journal, which only
            // records once the game has started. Keep the villain triggers so the
            // Nine card's flip-instead-of-destroy still applies.
            SetupNineGame();
            StartGame();

            // If The Siberian was deployed adjacent to Jack, he'd be immune.
            ReturnSiberian();

            PutMemberInPlay("JackSlashCharacter");
            DealDamage(jackslash, jackslash, 30, DamageType.Melee);
            AssertFlipped(jackslash);

            // Silence the deployed members' trash reactions (Bonesaw would heal
            // Jack above 10 when this defence card enters the trash).
            RemoveVillainTriggers();

            PlayCard("TheyreAllBetterNow");

            AssertNotFlipped(jackslash);
            AssertHitPoints(jackslash, 10);
        }

        [Test()]
        public void TestPlaysTopCardWhenNoDefeatedNine()
        {
            SetupNineGame();

            var legendary = StackDeck("Legendary");

            PlayCard("TheyreAllBetterNow");

            AssertIsInPlay(legendary);
        }
    }
}
