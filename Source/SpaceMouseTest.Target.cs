// Copyright 2018-2021 David Morasz All Rights Reserved.
// This source code is under MIT License https://github.com/microdee/UE4-SpaceMouse/blob/master/LICENSE

using UnrealBuildTool;
using System.Collections.Generic;

public class SpaceMouseTestTarget : TargetRules
{
	public SpaceMouseTestTarget(TargetInfo Target) : base(Target)
	{
		Type = TargetType.Game;
		DefaultBuildSettings = BuildSettingsVersion.Latest;
		ExtraModuleNames.AddRange( new string[] { "SpaceMouseTest" } );
	}
}
