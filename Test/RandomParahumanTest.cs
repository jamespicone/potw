using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Handelabra;

namespace Jp.ParahumansOfTheWormverse.UnitTest
{

    public class RandomParahumanTest : RandomGameTest
    {
        protected GameController SetupRandomParahumanTest(
            string overrideEnvironment = null,
            List<string> useHeroes = null,
            Dictionary<string, string> overrideVariants = null,
            string overrideVillain = null
        )
        {
            return SetupRandomGameController(
                reasonable: false,
                availableHeroes: ParahumansHeroes.Concat(DeckDefinition.AvailableHeroes),
                availableVillains: ParahumansVillains.Concat(DeckDefinition.AvailableVillains),
                availableEnvironments: ParahumansEnvironments.Concat(DeckDefinition.AvailableEnvironments),
                overrideEnvironment: overrideEnvironment,
                useHeroes: useHeroes,
                randomizeUseHeroes: true,
                overrideVariants: overrideVariants,
                overrideVillain: overrideVillain
            );
        }

        protected GameController SetupRandomParahumanTestWithTribunal(
            List<string> useHeroes = null,
            Dictionary<string, string> overrideVariants = null,
            string overrideVillain = null
        )
        {
            return SetupRandomGameController(
                reasonable: false,
                availableHeroes: ParahumansHeroes.Concat(DeckDefinition.AvailableHeroes),
                availableVillains: ParahumansVillains.Concat(DeckDefinition.AvailableVillains),
                availableEnvironments: ParahumansEnvironments.Concat(DeckDefinition.AvailableEnvironments),
                overrideEnvironment: "TheCelestialTribunal",
                useHeroes: useHeroes,
                randomizeUseHeroes: true,
                overrideVariants: overrideVariants,
                overrideVillain: overrideVillain
            );
        }

        protected GameController SetupRandomParahumanTestWithGuise(
            string overrideEnvironment = null,
            List<string> useHeroes = null,
            Dictionary<string, string> overrideVariants = null,
            string overrideVillain = null
        )
        {
            if (useHeroes == null) useHeroes = new List<string>();
            if (overrideVariants == null) overrideVariants = new Dictionary<string, string>();

            Assert.That(useHeroes.Count, Is.LessThan(5));

            useHeroes.Add("Guise");
            overrideVariants["Guise"] = "GuiseCharacter";

            return SetupRandomGameController(
                reasonable: false,
                availableHeroes: ParahumansHeroes.Concat(DeckDefinition.AvailableHeroes),
                availableVillains: ParahumansVillains.Concat(DeckDefinition.AvailableVillains),
                availableEnvironments: ParahumansEnvironments.Concat(DeckDefinition.AvailableEnvironments),
                overrideEnvironment: overrideEnvironment,
                useHeroes: useHeroes,
                randomizeUseHeroes: false,
                overrideVariants: overrideVariants,
                overrideVillain: overrideVillain
            );
        }

        protected GameController SetupRandomParahumanTestWithCompletionistGuise(
            string overrideEnvironment = null,
            List<string> useHeroes = null,
            Dictionary<string, string> overrideVariants = null,
            string overrideVillain = null
        )
        {
            if (useHeroes == null) useHeroes = new List<string>();
            if (overrideVariants == null) overrideVariants = new Dictionary<string, string>();

            Assert.That(useHeroes.Count, Is.LessThan(5));

            useHeroes.Add("Guise");
            overrideVariants["Guise"] = "CompletionistGuiseCharacter";

            return SetupRandomGameController(
                reasonable: false,
                availableHeroes: ParahumansHeroes.Concat(DeckDefinition.AvailableHeroes),
                availableVillains: ParahumansVillains.Concat(DeckDefinition.AvailableVillains),
                availableEnvironments: ParahumansEnvironments.Concat(DeckDefinition.AvailableEnvironments),
                overrideEnvironment: overrideEnvironment,
                useHeroes: useHeroes,
                randomizeUseHeroes: true,
                overrideVariants: overrideVariants,
                overrideVillain: overrideVillain
            );
        }

        protected GameController SetupRandomParahumanTestWithPWTempest(
            string overrideEnvironment = null,
            List<string> useHeroes = null,
            Dictionary<string, string> overrideVariants = null,
            string overrideVillain = null
        )
        {
            if (useHeroes == null) useHeroes = new List<string>();
            if (overrideVariants == null) overrideVariants = new Dictionary<string, string>();

            Assert.That(useHeroes.Count, Is.LessThan(5));

            useHeroes.Add("Tempest");
            overrideVariants["Tempest"] = "PrimeWardensTempestCharacter";

            return SetupRandomGameController(
                reasonable: false,
                availableHeroes: ParahumansHeroes.Concat(DeckDefinition.AvailableHeroes),
                availableVillains: ParahumansVillains.Concat(DeckDefinition.AvailableVillains),
                availableEnvironments: ParahumansEnvironments.Concat(DeckDefinition.AvailableEnvironments),
                overrideEnvironment: overrideEnvironment,
                useHeroes: useHeroes,
                randomizeUseHeroes: true,
                overrideVariants: overrideVariants,
                overrideVillain: overrideVillain
            );
        }

        protected GameController SetupRandomParahumanTestWithLabyrinth(
            string overrideEnvironment = null,
            List<string> useHeroes = null,
            Dictionary<string, string> overrideVariants = null,
            string overrideVillain = null
        )
        {
            if (useHeroes == null) useHeroes = new List<string>();
            if (overrideVariants == null) overrideVariants = new Dictionary<string, string>();

            Assert.That(useHeroes.Count, Is.LessThan(5));
            useHeroes.Add("Jp.ParahumansOfTheWormverse.Labyrinth");
            overrideVariants["Jp.ParahumansOfTheWormverse.Labyrinth"] = "LabyrinthCharacter";

            return SetupRandomGameController(
                reasonable: false,
                availableHeroes: ParahumansHeroes.Concat(DeckDefinition.AvailableHeroes),
                availableVillains: ParahumansVillains.Concat(DeckDefinition.AvailableVillains),
                availableEnvironments: ParahumansEnvironments.Concat(DeckDefinition.AvailableEnvironments),
                overrideEnvironment: overrideEnvironment,
                useHeroes: useHeroes,
                randomizeUseHeroes: true,
                overrideVariants: overrideVariants,
                overrideVillain: overrideVillain
            );
        }
    }
}