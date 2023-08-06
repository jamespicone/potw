﻿using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.UnitTest;

namespace Jp.ParahumansOfTheWormverse.UnitTest.Lung
{
    [TestFixture()]
    public class ThinkerCountermeasuresTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.TheSimurgh", "Tempest", "InsulaPrimalis");
        }
    }
}
