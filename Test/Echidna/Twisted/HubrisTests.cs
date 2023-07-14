using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Echidna
{
    [TestFixture()]
    public class HubrisTests : ParahumanTest
    {
        [Test()]
        public void TestImmuneToOneHit()
        {
            SetupGameController(
                "Jp.ParahumansOfTheWormverse.Echidna",
                "Jp.ParahumansOfTheWormverse.Alexandria",
                "Jp.ParahumansOfTheWormverse.Bitch",
                "InsulaPrimalis"
            );

            StartGame();
            DestroyNonCharacterVillainCards();
            ReturnAllTwisted();

            var hubris = PlayCard("HubrisTwisted");
            QuickHPStorage(hubris);
            DealDamage(alexandria, hubris, 3, DamageType.Melee);
            QuickHPCheck(0);
            DealDamage(alexandria, hubris, 3, DamageType.Melee);
            QuickHPCheck(-3);

            StackDeck("ChimaericalNightmare");
            StackDeck("ObsidianField");

            GoToEndOfTurn(FindEnvironment());

            DealDamage(alexandria, hubris, 1, DamageType.Melee);
            QuickHPCheck(-1);

            EnterNextTurnPhase();

            DealDamage(alexandria, hubris, 1, DamageType.Melee);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestDealsDamage()
        {
            SetupGameController(
                            "Jp.ParahumansOfTheWormverse.Echidna",
                            "Jp.ParahumansOfTheWormverse.Alexandria",
                            "Jp.ParahumansOfTheWormverse.Bitch",
                            "InsulaPrimalis"
                        );

            StartGame();
            DestroyNonCharacterVillainCards();
            ReturnAllTwisted();

            PlayCard("HubrisTwisted");

            StackDeck("ChimaericalNightmare");

            var effect = new CannotDealDamageStatusEffect();
            effect.SourceCriteria.IsSpecificCard = echidna.CharacterCard;
            RunCoroutine(GameController.AddStatusEffect(effect, false, echidna.CharacterCardController.GetCardSource()));

            QuickHPStorage(alexandria, bitch);
            GoToPlayCardPhase(echidna);
            QuickHPCheck(0, 0);
            GoToEndOfTurn(echidna);
            QuickHPCheck(-1, -1);
        }
    }
}
