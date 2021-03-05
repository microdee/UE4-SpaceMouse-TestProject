## Test project for SpaceMouse plugin for UE4

# [Go to the plugin repository](https://github.com/microdee/UE4-SpaceMouse)

## Build SpaceMouse for Unreal Engine

If you only want to use it in your project then you can just submodule https://github.com/microdee/UE4-SpaceMouse.git into the project `Plugins` folder then ignore the rest of this guide.

If you'd like to develop it or make an Engine plugin for older versions of Unreal, carry on reading:

SpaceMouse for Unreal Engine uses [Nuke](https://nuke.build) with [Nuke.Unreal](https://github.com/microdee/Nuke.Unreal) to automate build tasks and chores usually associated with developing an Unreal project.

### Build editor for developing the plugin

Just run `build.cmd` without arguments. If it complains about missing .NET 5 then install .NET 5 SDK too.

If you need to develop for a different Unreal Engine version than the default, execute

```
> .\build.cmd --target Checkout --unreal-version 4.25
```

If you have Nuke installed as a global dotnet tool you can avoid `--target` and execute the command from any subfolder:

```
> nuke Checkout --unreal-version 4.25
```

### Make releases

```
> .\build.cmd --target MakeRelease --unreal-version 4.26.1 --no-logo
```

If you have Nuke installed as a global dotnet tool you can avoid `--target` and execute the command from any subfolder:

```
> nuke MakeRelease --unreal-version 4.26.1 --no-logo
```

To prepare a marketplace compliant release add `--for-marketplace` argument.

Resulting archives are placed in `/.deploy` folder by default.

### Troubleshooting

**If Nuke complains about not finding your Unreal engine folder, then**

1. Open `/Nuke.Targets/Build.cs`
2. In the `Build` class replace the line:
```CSharp
public static int Main() => Execute<Build>(x => x.BuildEditor);
```
with
```CSharp
public static int Main()
{
    Unreal.EngineSearchPaths.Add((AbsolutePath) @"C:\My\Special\Path");
    return Execute<Build>(x => x.BuildEditor);
}
```
3. **NOTE** that the path you provide should be the parent path to your Unreal engine versions (typically they're in a folder like UE_4.26)

If for any ungodly reason the actual folder of your Unreal Engine installation doesn't look like UE_4.26 (`UE_{major}.{minor}`) you can provide a new format as an argument:

```
nuke MakeRelease --unreal-subfolder MySpecialUE_{0}.{1}
```

or if you want to make that permanent you can edit the `Build` class in `/Nuke.Targets/Build.cs` by adding

```CSharp
public override string UnrealSubFolder => "MySpecialUE_{0}.{1}"
```