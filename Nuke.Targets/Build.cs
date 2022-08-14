using Nuke.Unreal;

class Build : UnrealBuild, IPluginTargets
{
    public static int Main() => Execute<Build>(x => x.BuildEditor);
    protected override void OnBuildCreated() => NoLogo = true;
    
    public string PluginVersion => "1.2.5";
}
