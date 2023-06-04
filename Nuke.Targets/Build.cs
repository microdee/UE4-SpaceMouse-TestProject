using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities.Collections;
using Nuke.Unreal;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

class Build : UnrealBuild, IPluginTargets
{
    public static int Main () => Execute<Build>(x => x.Generate);
    protected override void OnBuildCreated() => NoLogo = true;
    public string PluginVersion => "1.2.6";
}
