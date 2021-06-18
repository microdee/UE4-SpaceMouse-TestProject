using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities.Collections;
using Nuke.Unreal;
using Scriban;

using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Logger;

partial class Build : PluginTargets
{
    record Property(
        string Name,
        string NlType,
        string UeType
    ) {}

    static List<Property> NlProperties = new() {
        new("Active"                , "bool"     , "bool"),
        new("Focus"                 , "bool"     , "bool"),
        new("Motion"                , "bool"     , "bool"),
        // new("CoordinateSystem"      , "matrix"   , "FMatrix"),
        new("DevicePresent"         , "bool"     , "bool"),
        new("EventsKeyPress"        , "long"     , "int"),
        new("EventsKeyRelease"      , "long"     , "int"),
        new("Transaction"           , "long"     , "int"),
        // new("FrameTime"             , "double"   , "float"),
        // new("FrameTimingSource"     , "long"     , "int"),
        new("ViewAffine"            , "matrix"   , "FMatrix"),
        new("ViewConstructionPlane" , "plane"    , "FPlane"),
        new("ViewExtents"           , "box"      , "FBox"),
        new("ViewFov"               , "float"    , "float"),
        new("ViewFrustum"           , "frustum"  , "FMatrix"),
        new("ViewPerspective"       , "bool"     , "bool"),
        new("ViewRotatable"         , "bool"     , "bool"),
        new("ViewTarget"            , "point"    , "FVector"),
        new("ViewsFront"            , "matrix"   , "FMatrix"),
        new("PivotPosition"         , "point"    , "FVector"),
        new("PivotUser"             , "bool"     , "bool"),
        new("PivotVisible"          , "bool"     , "bool"),
        new("HitLookfrom"           , "point"    , "FVector"),
        new("HitDirection"          , "vector"   , "FVector"),
        new("HitAperture"           , "float"    , "float"),
        new("HitLookat"             , "point"    , "FVector"),
        new("HitSelectionOnly"      , "bool"     , "bool"),
        new("SelectionAffine"       , "matrix"   , "FMatrix"),
        new("SelectionEmpty"        , "bool"     , "bool"),
        new("SelectionExtents"      , "box"      , "FBox"),
        new("ModelExtents"          , "box"      , "FBox"),
        new("PointerPosition"       , "point"    , "FVector"),
        // new("CommandsActiveSet"     , "string_t" , "FString"),
        // new("CommandsActiveCommand" , "string_t" , "FString"),
        // new("Settings"              , "string_t" , "FString"),
        // new("SettingsChanged"       , "long"     , "int"),
        // new("Images"                , "imagearray_t"      , "imagearray_t"),
        // new("CommandsTree"          , "SiActionNodeEx_t*" , "SiActionNodeEx_t*"),
    };

    public Target GenerateRuntimeNavlibInterface => _ => _
        .Executes(() => {
            var smRuntimeDir = ToPlugin.Parent / "Source" / "SpaceMouseRuntime";
            var headerInputPath = smRuntimeDir / "Public" / "SmNavContext.sbnh";
            var headerOutputPath = smRuntimeDir / "Public" / "SmNavContext.h";
            var cppInputPath = smRuntimeDir / "Private" / "SmNavContext.sbncpp";
            var cppOutputPath = smRuntimeDir / "Private" / "SmNavContext.cpp";

            var template = Template.Parse(File.ReadAllText(headerInputPath));
            var result = template.Render(new {
                Properties = NlProperties
            });
            File.WriteAllText(headerOutputPath, result);

            template = Template.Parse(File.ReadAllText(cppInputPath));
            result = template.Render(new {
                Properties = NlProperties
            });
            File.WriteAllText(cppOutputPath, result);
        });
}