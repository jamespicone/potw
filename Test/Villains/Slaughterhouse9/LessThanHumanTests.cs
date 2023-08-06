﻿using NUnit.Framework;
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
    public class LessThanHumanTests : ParahumanTest
    {
        [Test()]
        public void TestModWorks()
        {
            SetupGameController("Jp.ParahumansOfTheWormverse.Slaughterhouse9", "Tempest", "InsulaPrimalis");
        }
    }
}
