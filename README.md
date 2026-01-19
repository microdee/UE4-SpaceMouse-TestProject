<img src="Docs/Images/SpacePro-Thumb-2.0-OnLight.png#gh-light-mode-only" />
<img src="Docs/Images/SpacePro-Thumb-2.0-OnDark.png#gh-dark-mode-only" />

## Test project for OpenUnrealSpaceMouse

# [Go to the plugin repository](https://github.com/microdee/OpenUnrealSpaceMouse)

## Build OpenUnrealSpaceMouse

OpenUnrealSpaceMouse uses [Nuke](https://nuke.build) with [Nuke.Unreal](https://mcro.de/Nuke.Unreal) to automate build tasks and chores usually associated with developing an Unreal project. It also uses [MCRO](https://mcro.de/mcro/) for various C++ utilities.

### With your own project

1. Set up [Nuke.Unreal](https://mcro.de/Nuke.Unreal/d8/d51/GettingStarted.html) for your project
2. Submodule https://github.com/microdee/mcro.git into the project `Plugins` folder
3. Submodule https://github.com/microdee/OpenUnrealSpaceMouse.git into the project `Plugins` folder.
   * (or copy sources if you use Perforce)
4. In a terminal do
   ```
   nuke generate
   ```

### For distribution

Clone this repository and follow above steps starting at step 2. To make a release do the following in a terminal:

```
nuke switch --unreal 5.7; nuke BuildOuesm
```

Resulting archives are placed in `/.deploy` folder by default. Please refer to [Nuke.Unreal](https://mcro.de/Nuke.Unreal) documentation for more control and/or customization.
