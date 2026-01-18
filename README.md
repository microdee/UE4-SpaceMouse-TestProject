<img src="Docs/Images/SpacePro-Thumb-2.0-OnLight.png#gh-light-mode-only" />
<img src="Docs/Images/SpacePro-Thumb-2.0-OnDark.png#gh-dark-mode-only" />

## Test project for OpenUnrealSpaceMouse

# [Go to the plugin repository](https://github.com/microdee/OpenUnrealSpaceMouse)

## Build OpenUnrealSpaceMouse

If you only want to use it in your project then you can just submodule https://github.com/microdee/OpenUnrealSpaceMouse.git into the project `Plugins` folder then ignore the rest of this guide.

If you'd like to develop it or make an Engine plugin for older versions of Unreal, carry on reading:

OpenUnrealSpaceMouse uses [Nuke](https://nuke.build) with [Nuke.Unreal](https://mcro.de/Nuke.Unreal) to automate build tasks and chores usually associated with developing an Unreal project. It is recommended to have the Nuke global tool installed.

### Make releases

```
> nuke switch --unreal 5.7; nuke BuildOuesm
```

Resulting archives are placed in `/.deploy` folder by default. Please refer to [Nuke.Unreal](https://mcro.de/Nuke.Unreal) documentation for more control and/or customization.