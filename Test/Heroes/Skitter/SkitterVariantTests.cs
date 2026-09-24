using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Jp.ParahumansOfTheWormverse.Skitter;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Skitter
{
    [TestFixture()]
    public class SkitterWeaverTests : ParahumanTest
    {
        private const string Weaver = "Jp.ParahumansOfTheWormverse.Skitter/SkitterWeaverCharacter";

        [Test()]
        public void TestFirstStrategyEachRoundLetsAnotherPlayerPlay()
        {
            SetupGameController("BaronBlade", Weaver, "Bunker", "Megalopolis");
            StartGame();

            var plating = PutInHand("HeavyPlating");
            var maintenance = PutInHand("MaintenanceUnit");
            DecisionSelectTurnTaker = bunker.TurnTaker;
            DecisionSelectCardToPlay = plating;

            PlayCard("DeliveryService");
            AssertIsInPlay(plating);

            // Second Strategy in the same round does nothing.
            DecisionSelectCardToPlay = maintenance;
            PlayCard("TrackingBugs");
            AssertInHand(maintenance);

            // Next round it works again.
            GoToStartOfTurn(baron);
            GoToStartOfTurn(skitter);
            PlayCard("UnrelentingStings");
            AssertIsInPlay(maintenance);
        }

        [Test()]
        public void TestNonStrategyDoesNotTrigger()
        {
            SetupGameController("BaronBlade", Weaver, "Bunker", "Megalopolis");
            StartGame();

            var plating = PutInHand("HeavyPlating");
            DecisionSelectCardToPlay = plating;

            PlayCard("SpidersilkArmour");
            AssertInHand(plating);
        }

        [Test()]
        public void TestPowerOtherPlayerDraws()
        {
            SetupGameController("BaronBlade", Weaver, "Bunker", "Megalopolis");
            StartGame();

            PlayCard("DeliveryService");

            DecisionSelectFunction = 0;
            QuickHandStorage(skitter, bunker);
            UsePower(skitter);
            QuickHandCheck(0, 1);
        }

        [Test()]
        public void TestPowerPlacesTokensOnTwoStrategies()
        {
            SetupGameController("BaronBlade", Weaver, "Bunker", "Megalopolis");
            StartGame();

            var delivery = PlayCard("DeliveryService");
            var tracking = PlayCard("TrackingBugs");
            var stings = PlayCard("UnrelentingStings");

            DecisionSelectFunction = 1;
            DecisionSelectCards = new Card[] { delivery, stings };
            QuickHandStorage(skitter, bunker);
            UsePower(skitter);
            QuickHandCheck(0, 0);

            AssertTokenPoolCount(delivery.FindBugPool(), 1);
            AssertTokenPoolCount(tracking.FindBugPool(), 0);
            AssertTokenPoolCount(stings.FindBugPool(), 1);
            AssertTokenPoolCount(skitter.CharacterCard.FindBugPool(), 0);
        }

        [Test()]
        public void TestIncapReducesNextDamage()
        {
            SetupGameController("BaronBlade", Weaver, "Bunker", "Megalopolis");
            StartGame();
            IncapacitateCharacter(skitter.CharacterCard, baron.CharacterCard);

            DecisionSelectCard = baron.CharacterCard;
            UseIncapacitatedAbility(skitter, 0);

            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 3, DamageType.Melee);
            QuickHPCheck(-1);
            DealDamage(baron, bunker, 3, DamageType.Melee);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestIncapPlayerPlaysCard()
        {
            SetupGameController("BaronBlade", Weaver, "Bunker", "Megalopolis");
            StartGame();
            IncapacitateCharacter(skitter.CharacterCard, baron.CharacterCard);

            var plating = PutInHand("HeavyPlating");
            DecisionSelectTurnTaker = bunker.TurnTaker;
            DecisionSelectCardToPlay = plating;
            UseIncapacitatedAbility(skitter, 1);
            AssertIsInPlay(plating);
        }

        [Test()]
        public void TestIncapRevealAndDiscard()
        {
            SetupGameController("BaronBlade", Weaver, "Bunker", "Megalopolis");
            StartGame();
            IncapacitateCharacter(skitter.CharacterCard, baron.CharacterCard);

            var top = baron.TurnTaker.Deck.TopCard;
            DecisionSelectLocation = new LocationChoice(baron.TurnTaker.Deck);
            DecisionMoveCardDestination = new MoveCardDestination(baron.TurnTaker.Trash);
            UseIncapacitatedAbility(skitter, 2);
            AssertInTrash(top);
        }

        [Test()]
        public void TestIncapRevealAndReplace()
        {
            SetupGameController("BaronBlade", Weaver, "Bunker", "Megalopolis");
            StartGame();
            IncapacitateCharacter(skitter.CharacterCard, baron.CharacterCard);

            var top = baron.TurnTaker.Deck.TopCard;
            DecisionSelectLocation = new LocationChoice(baron.TurnTaker.Deck);
            DecisionMoveCardDestination = new MoveCardDestination(baron.TurnTaker.Deck);
            UseIncapacitatedAbility(skitter, 2);
            Assert.That(baron.TurnTaker.Deck.TopCard, Is.EqualTo(top));
        }

        [Test()]
        public void TestBroughtInByRepresentativeOfEarth()
        {
            SetupGameController("BaronBlade", "Legacy", "TheCelestialTribunal");
            StartGame();

            var weaver = SummonRepresentativeOfEarth("Skitter", "SkitterWeaverCharacter");
            QuickHandStorage(legacy);
            UsePower(weaver, 0);
            QuickHandCheck(1);
        }
    }

    [TestFixture()]
    public class SkitterTaylorHebertTests : ParahumanTest
    {
        private const string Taylor = "Jp.ParahumansOfTheWormverse.Skitter/SkitterTaylorHebertCharacter";

        [Test()]
        public void TestPlayingBugDraws()
        {
            SetupGameController("BaronBlade", Taylor, "Bunker", "Megalopolis");
            StartGame();

            QuickHandStorage(skitter);
            PlayCard("SwarmOfFlies");
            QuickHandCheck(1);

            PlayCard("DeliveryService");
            QuickHandCheck(0);
        }

        [Test()]
        public void TestPowerDiscardsForTokens()
        {
            SetupGameController("BaronBlade", Taylor, "Bunker", "Megalopolis");
            StartGame();

            var hand = skitter.HeroTurnTaker.Hand.Cards.Take(2).ToArray();
            DecisionSelectCards = new Card[] { hand[0], hand[1], null };

            QuickHandStorage(skitter);
            UsePower(skitter);
            QuickHandCheck(-1);
            AssertInTrash(hand);
            AssertTokenPoolCount(skitter.CharacterCard.FindBugPool(), 2);
        }

        [Test()]
        public void TestPowerDiscardNothing()
        {
            SetupGameController("BaronBlade", Taylor, "Bunker", "Megalopolis");
            StartGame();

            DecisionSelectCards = new Card[] { null };

            QuickHandStorage(skitter);
            UsePower(skitter);
            QuickHandCheck(1);
            AssertTokenPoolCount(skitter.CharacterCard.FindBugPool(), 0);
        }

        [Test()]
        public void TestIncapEnvironmentDamagesEachTarget()
        {
            SetupGameController("BaronBlade", Taylor, "Bunker", "Megalopolis");
            StartGame();
            IncapacitateCharacter(skitter.CharacterCard, baron.CharacterCard);

            var mdp = GetMobileDefensePlatform().Card;
            QuickHPStorage(mdp, bunker.CharacterCard);
            UseIncapacitatedAbility(skitter, 0);
            QuickHPCheck(-1, -1);
        }

        [Test()]
        public void TestIncapDestroyOngoing()
        {
            SetupGameController("BaronBlade", Taylor, "Bunker", "Megalopolis");
            StartGame();
            IncapacitateCharacter(skitter.CharacterCard, baron.CharacterCard);

            var ammo = PlayCard("AmmoDrop");
            DecisionSelectCard = ammo;
            UseIncapacitatedAbility(skitter, 1);
            AssertInTrash(ammo);
        }

        [Test()]
        public void TestIncapDiscardToDrawTwo()
        {
            SetupGameController("BaronBlade", Taylor, "Bunker", "Megalopolis");
            StartGame();
            IncapacitateCharacter(skitter.CharacterCard, baron.CharacterCard);

            var discard = bunker.HeroTurnTaker.Hand.Cards.First();
            DecisionSelectTurnTaker = bunker.TurnTaker;
            DecisionSelectCard = discard;
            DecisionYesNo = true;

            QuickHandStorage(bunker);
            UseIncapacitatedAbility(skitter, 2);
            QuickHandCheck(1);
            AssertInTrash(discard);
        }

        [Test()]
        public void TestIncapDeclineDiscard()
        {
            SetupGameController("BaronBlade", Taylor, "Bunker", "Megalopolis");
            StartGame();
            IncapacitateCharacter(skitter.CharacterCard, baron.CharacterCard);

            DecisionSelectTurnTaker = bunker.TurnTaker;
            DecisionYesNo = false;

            QuickHandStorage(bunker);
            UseIncapacitatedAbility(skitter, 2);
            QuickHandCheck(0);
        }

        [Test()]
        public void TestBroughtInByRepresentativeOfEarth()
        {
            SetupGameController("BaronBlade", "Legacy", "TheCelestialTribunal");
            StartGame();

            var taylor = SummonRepresentativeOfEarth("Skitter", "SkitterTaylorHebertCharacter");
            UsePower(taylor, 0);
        }
    }

    [TestFixture()]
    public class SkitterKhepriTests : ParahumanTest
    {
        private const string Khepri = "Jp.ParahumansOfTheWormverse.Skitter/SkitterKhepriCharacter";

        [Test()]
        public void TestTokenAtStartOfTurn()
        {
            SetupGameController("BaronBlade", Khepri, "Bunker", "Megalopolis");
            StartGame();

            var pool = skitter.CharacterCard.FindBugPool();
            AssertTokenPoolCount(pool, 0);
            GoToStartOfTurn(skitter);
            AssertTokenPoolCount(pool, 1);
            GoToStartOfTurn(bunker);
            AssertTokenPoolCount(pool, 1);
        }

        [Test()]
        public void TestPowerRemoveToken()
        {
            SetupGameController("BaronBlade", Khepri, "Bunker", "Megalopolis");
            StartGame();

            var pool = skitter.CharacterCard.FindBugPool();
            pool.AddTokens(2);
            var delivery = PlayCard("DeliveryService");

            DecisionSelectFunction = 0;
            DecisionSelectCard = skitter.CharacterCard;
            QuickHPStorage(bunker);
            QuickHandStorage(bunker);
            UsePower(skitter);
            QuickHandCheck(1);
            QuickHPCheck(0);
            AssertTokenPoolCount(pool, 1);
            AssertIsInPlay(delivery);
        }

        [Test()]
        public void TestPowerRemoveCard()
        {
            SetupGameController("BaronBlade", Khepri, "Bunker", "Megalopolis");
            StartGame();

            var pool = skitter.CharacterCard.FindBugPool();
            pool.AddTokens(2);
            var delivery = PlayCard("DeliveryService");

            DecisionSelectFunction = 1;
            DecisionSelectCard = delivery;
            QuickHPStorage(bunker);
            QuickHandStorage(bunker);
            UsePower(skitter);
            QuickHandCheck(1);
            QuickHPCheck(-2);
            AssertTokenPoolCount(pool, 2);
            AssertOutOfGame(delivery);
        }

        [Test()]
        public void TestPowerMustRemoveCardWithNoTokens()
        {
            SetupGameController("BaronBlade", Khepri, "Bunker", "Megalopolis");
            StartGame();

            var delivery = PlayCard("DeliveryService");

            QuickHPStorage(bunker);
            UsePower(skitter);
            QuickHPCheck(-2);
            AssertOutOfGame(delivery);
        }

        [Test()]
        public void TestPowerNothingToRemove()
        {
            SetupGameController("BaronBlade", Khepri, "Bunker", "Megalopolis");
            StartGame();

            QuickHPStorage(bunker);
            QuickHandStorage(bunker);
            UsePower(skitter);
            QuickHandCheck(1);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestIncapSacrifice()
        {
            SetupGameController("BaronBlade", Khepri, "Bunker", "Megalopolis");
            StartGame();
            IncapacitateCharacter(skitter.CharacterCard, baron.CharacterCard);

            var maintenance = PlayCard("MaintenanceUnit");
            DecisionSelectTurnTaker = bunker.TurnTaker;
            DecisionSelectCard = maintenance;

            QuickHandStorage(bunker);
            UseIncapacitatedAbility(skitter, 1);
            QuickHandCheck(1);
            AssertInTrash(maintenance);

            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 3, DamageType.Melee);
            QuickHPCheck(0);
            DealDamage(baron, bunker, 3, DamageType.Melee);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestIncapPuppet()
        {
            SetupGameController("BaronBlade", Khepri, "Bunker", "Megalopolis");
            StartGame();
            IncapacitateCharacter(skitter.CharacterCard, baron.CharacterCard);

            var mdp = GetMobileDefensePlatform().Card;
            DecisionSelectCard = mdp;
            DecisionSelectTarget = bunker.CharacterCard;

            QuickHPStorage(mdp, bunker.CharacterCard);
            UseIncapacitatedAbility(skitter, 2);
            QuickHPCheck(-1, -1);
        }

        [Test()]
        public void TestBroughtInByRepresentativeOfEarth()
        {
            SetupGameController("BaronBlade", "Legacy", "TheCelestialTribunal");
            StartGame();

            var khepri = SummonRepresentativeOfEarth("Skitter", "SkitterKhepriCharacter");
            UsePower(khepri, 0);
        }
    }
}
