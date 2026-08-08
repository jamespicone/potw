using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Environment.Kyushu
{
    public abstract class KyushuTestBase : ParahumanTest
    {
        // Baron Blade's character card stays in play as a 40 HP non-environment
        // target (the highest, unless a test says otherwise).
        protected void SetupKyushuGame()
        {
            SetupGameController("BaronBlade", "Legacy", "Bunker", "Haka", "Jp.ParahumansOfTheWormverse.Kyushu");
            StartGame();
            RemoveVillainCards();
            RemoveVillainTriggers();
        }
    }
}
