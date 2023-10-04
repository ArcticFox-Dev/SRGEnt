using a;
using NuGet.Versioning;
using Nuke.Common;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Serilog;

class Build : NukeBuild
{
    [LatestGitHubRelease(
        identifier: "ArcticFox-Dev/SRGEnt",
        TrimPrefix = true)]
    readonly string GithubRelease;
    
    [LatestNuGetVersion(
        packageId: "SRGEnt",
        IncludePrerelease = true)]
    readonly NuGetVersion NuGetVersion;
    
    [Solution(GenerateProjects = true)]
    readonly Solution Solution;

    public static int Main () => Execute<Build>(x => x.Test);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    NuGetVersion _version;
    
    Target Clean => run => run
        .Before(Restore)
        .Executes(() =>
        {
            DotNetTasks.DotNetClean(settings =>
                settings.SetProcessWorkingDirectory(RootDirectory));
        });

    Target Restore => run => run
        .Executes(() =>
        {
            DotNetTasks.DotNetRestore(settings =>
                settings.SetProcessWorkingDirectory(RootDirectory));
        });

    Target Test => run => run
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetTasks.DotNetTest(setting =>
                setting.SetProcessWorkingDirectory(RootDirectory));
        });

    Target Get_Version => _ => _
        .Executes(() =>
        {
            if (NuGetVersion != null)
            {
                _version = NuGetVersion;
            }
            else
            {
                Assert.True(NuGetVersion.TryParseStrict(GithubRelease, out _version), "Failed to resolve package version.");
            }

            _version = _version.Increment(NuGetVersionElement.Patch);
            Log.Information($"Resolved version as: {_version}");
        });

    Target Test_Core => run => run
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetTasks.DotNetTest(settings =>
                settings.SetProjectFile(Solution.SRGEnt_Core_Tests.Path));
        });
    Target Package_Core => run => run
        .DependsOn(Test_Core, Get_Version)
        .Executes(() =>
        {
            DotNetTasks.DotNetPack(settings => settings
                .SetProcessWorkingDirectory(Solution.SRGEnt_Core.Directory)
                .SetVersion(_version.ToNormalizedString())
                .SetConfiguration(Configuration.Release));
        });
    
    Target Publish_Core => _ => _
        .DependsOn(Package_Core)
        .Executes(() =>
        { });

    Target Test_Generator => run => run
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetTasks.DotNetTest(settings => settings
                .SetProjectFile(Solution.SRGEnt_Generator_Tests.Path));
        });

    Target Package_Generator => run => run
        .DependsOn(Test_Generator, Get_Version)
        .Executes(() =>
        {
            DotNetTasks.DotNetPack(settings => settings
                .SetProcessWorkingDirectory(Solution.SRGEnt_Generator.Directory)
                .SetVersion(_version.ToNormalizedString())
                .SetConfiguration(Configuration.Release));
        });

    Target Publish_Generator => _ => _
        .DependsOn(Package_Generator)
        .Executes(() =>
        { });

    Target PackForNuGet => run => run
        .DependsOn(Package_Core, Package_Generator);

    Target PublishCorePackages => _ => _
        .DependsOn(Publish_Core, Publish_Generator);
    
    Target NuGet_Publish => run => run
        .DependsOn(PublishCorePackages)
        .Executes(() =>
        {
            DotNetTasks.DotNet($"add package SRGEnt.Core -v {_version.ToNormalizedString()}", Solution.SRGEnt.Directory);
            DotNetTasks.DotNet($"add package SRGEnt.Generator -v {_version.ToNormalizedString()}", Solution.SRGEnt.Directory);
            Log.Information("Dependencies Added");
            DotNetTasks.DotNetPack(settings => settings
                .SetProcessWorkingDirectory(Solution.SRGEnt.Directory)
                .SetVersion(_version.ToNormalizedString())
                .SetConfiguration(Configuration.Release));
        });

}
