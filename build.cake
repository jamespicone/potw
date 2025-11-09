#tool nuget:?package=NUnit.ConsoleRunner&version=3.18.2

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var solution = "./ParahumansOfTheWormverse.sln";
var modProject = "./Mod/ParahumansOfTheWormverse.csproj";
var testProject = "./Test/ParahumansOfTheWormverseUnitTests.csproj";
var modOutputDir = "./ParahumansOfTheWormverse";
var buildArtifacts = "./BuildArtifacts";
var testResultsDir = "./TestResults";

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx =>
{
    Information("Running tasks...");
    Information("Configuration: {0}", configuration);
});

Teardown(ctx =>
{
    Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectories(new DirectoryPath[] {
        Directory("./Mod/bin"),
        Directory("./Mod/obj"),
        Directory("./Test/bin"),
        Directory("./Test/obj"),
        Directory(modOutputDir),
        Directory(buildArtifacts),
        Directory(testResultsDir)
    });

    Information("Cleaned build directories");
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    // Restore both projects using dotnet (both are now SDK-style)
    DotNetRestore(modProject);
    DotNetRestore(testProject);

    Information("NuGet packages restored");
});

Task("Build-Mod")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    DotNetBuild(modProject, new DotNetBuildSettings {
        Configuration = configuration,
        Verbosity = DotNetVerbosity.Minimal,
        MSBuildSettings = new DotNetMSBuildSettings()
            .WithProperty("SolutionDir", MakeAbsolute(Directory(".")).FullPath + "\\")
    });

    Information("Mod project built successfully");
});

Task("Build-Tests")
    .IsDependentOn("Build-Mod")
    .Does(() =>
{
    DotNetBuild(testProject, new DotNetBuildSettings {
        Configuration = configuration,
        Verbosity = DotNetVerbosity.Minimal,
        MSBuildSettings = new DotNetMSBuildSettings()
            .WithProperty("SolutionDir", MakeAbsolute(Directory(".")).FullPath + "\\")
    });

    Information("Test project built successfully");
});

Task("Build")
    .IsDependentOn("Build-Tests")
    .Does(() =>
{
    Information("All projects built successfully");
});

Task("Run-Tests")
    .IsDependentOn("Build-Tests")
    .Does(() =>
{
    // Ensure test results directory exists
    EnsureDirectoryExists(testResultsDir);

    DotNetTest(testProject, new DotNetTestSettings {
        Configuration = configuration,
        NoBuild = true,
        NoRestore = true,
        Verbosity = DotNetVerbosity.Normal,
        ArgumentCustomization = args => args.Append("--logger \"console;verbosity=detailed\"")
    });

    Information("Tests completed successfully");
});

Task("Package")
    .IsDependentOn("Build")
    .Does(() =>
{
    EnsureDirectoryExists(buildArtifacts);

    // The mod folder is already created by the build process
    // Let's create a zip of it for distribution
    if (DirectoryExists(modOutputDir))
    {
        var zipFile = $"{buildArtifacts}/ParahumansOfTheWormverse-{configuration}.zip";
        Zip(modOutputDir, zipFile);
        Information($"Mod packaged to: {zipFile}");
    }
    else
    {
        Warning($"Mod output directory not found: {modOutputDir}");
    }
});

Task("Rebuild")
    .IsDependentOn("Clean")
    .IsDependentOn("Build")
    .Does(() =>
{
    Information("Rebuild completed");
});

Task("Full")
    .IsDependentOn("Rebuild")
    .IsDependentOn("Run-Tests")
    .IsDependentOn("Package")
    .Does(() =>
{
    Information("Full build completed successfully");
});

Task("Default")
    .IsDependentOn("Build")
    .Does(() =>
{
    Information("Default build task completed");
});

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);
