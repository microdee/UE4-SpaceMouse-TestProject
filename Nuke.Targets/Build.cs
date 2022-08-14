using Nuke.Unreal;

class Build : UnrealBuild, IPluginTargets
{
    public static int Main() => Execute<Build>(x => x.BuildEditor);
    
    public string PluginVersion => "1.2.4";
}
