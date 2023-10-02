using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using YamlDotNet.Serialization;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

class Build : NukeBuild
{

    [Solution(GenerateProjects = true)]
    readonly Solution Solution;

    public static int Main () => Execute<Build>(x => x.Test);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

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

    Target Package_Core => run => run
        .DependsOn(Test)
        .Executes(() =>
        {
            DotNetTasks.DotNetPack(settings => settings
                .SetProcessWorkingDirectory(Solution.src.SRGEnt_Core.Directory)
                .SetConfiguration(Configuration.Release));
        });

    Target Package_Generator => run => run
        .DependsOn(Test)
        .Executes(() =>
        {
            DotNetTasks.DotNetPack(settings => settings
                .SetProcessWorkingDirectory(Solution.src.SRGEnt_Generator.Directory)
                .SetConfiguration(Configuration.Release));
        });

    Target PackForNuGet => run => run
        .DependsOn(Package_Core, Package_Generator);

}
