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
        string UeType,
        bool Read,
        bool Write,
        string Docs = ""
    ) {}

    static List<Property> NlProperties = new() {
        new("Active"                , "bool"     , "bool", true  , false ,
@"     * Specifies that the navigation instance is currently active.
     *
     * Clients that have multiple navigation instances open need to inform the navlib which of them is 
     * the target for 3D Mouse input. They do this by setting the active_k property of a navigation 
     * instance to true."),

        new("Focus"                 , "bool"     , "bool", true  , false ,
@"     * Specifies that the application has keyboard focus.
     *
     * Clients that run in container applications via the NLServer proxy set this property to indicate
     * keyboard focus. This will set 3DMouse focus to the navlib connection."),

        new("Motion"                , "bool"     , "bool", false , true  ,
@"     * Specifies that a motion model is active.
     *
     * The motion_k property is set to true by the navlib to notify the client that it is executing a
     * motion model and will update the camera matrix regularly. This is useful for clients that need
     * to run an animation loop. When the navlib has finished navigating the camera position it will
     * set the property to false. By setting motion_k to false, a client may temporarily interrupt a
     * navigation communication and forces the Navlib to reinitialize the navigation."),

        // new("CoordinateSystem"      , "matrix"   , "FMatrix", true, false),
        new("DevicePresent"         , "bool"     , "bool", false , true  ,
@"     * Specifies whether a device is present
     * Currently this always returns true."),

        new("EventsKeyPress"        , "long"     , "int", false , true  , "     * V3DK press event."),
        new("EventsKeyRelease"      , "long"     , "int", false , true  , "     * V3DK release event."),

        new("Transaction"           , "long"     , "int", false , true  ,
@"     * Specifies the navigation transaction.
     *
     * The Navigation Library can set more than one client property for a single navigation frame. For 
     * example when navigating in an orthographic projection possibly both the view affine and extents 
     * will be modified depending on the 3DMouse input. The Navigation Library will set the 
     * transaction_k property to a value >0 at the beginning of a navigation frame and to 0 at the end. 
     * Clients that need to actively refresh the view can trigger the refresh when the value is set to 0."),

        // new("FrameTime"             , "double"   , "float",  true  , false),
        // new("FrameTimingSource"     , "long"     , "int",  true  , false),
        new("ViewAffine"            , "matrix"   , "FMatrix", true  , true  ,
@"     * Specifies the matrix of the camera in the view.
     *
     * This matrix specifies the camera to world transformation of the view. That is, multiplying this 
     * matrix on the right by the position (0, 0, 0) yields the position of the camera in world coordinates. 
     * The navlib will, generally, query this matrix at the beginning of a navigation action and then set 
     * the property per frame. The frame rate that the navlib attempts to achieve is related to the 3D 
     * mouse event rate and is about 60Hz."),

        new("ViewConstructionPlane" , "plane"    , "FPlane", true  , false ,
@"     * Specifies the plane equation of the construction plane as a normal and a distance (general form 
     * of the equation of a plane).
     *
     * This property is used by the Navigation Library to distinguish views used for construction in an 
     * orthographic projection: typically the top, right left etc. views. The Navigation Library assumes 
     * that when the camera’s look-at axis is parallel to the plane normal the view should not be 
     * rotated."),

        new("ViewExtents"           , "box"      , "FBox", true  , true  ,
@"     * Specifies the orthographic extents the view in camera coordinates
     *
     * This orthographic extents of the view are returned as a bounding box in camera/view 
     * coordinates. The navlib will only access this property if the view is orthographic."),

        new("ViewFov"               , "float"    , "float", true  , true  , "     * Specifies the field-of-view of a perspective camera/view in radians"),

        new("ViewFrustum"           , "frustum"  , "FMatrix", true  , false ,
@"     * Specifies the frustum of a perspective camera/view in camera coordinates
     *
     * The navlib uses this property to calculate the field-of-view of the perspective camera. The 
     * frustum is also used in algorithms that need to determine if the model is currently visible. The
     * navlib will not write to this property. Instead, if necessary, the navlib will write to the view_fov_k 
     * property and leave the client to change the frustum as it wishes."),

        new("ViewPerspective"       , "bool"     , "bool", true  , false ,
@"     * Specifies the projection of the view/camera
     *
     * This property defaults to true. If the client does not supply a function for the navlib to query the 
     * view’s projection (which it will generally do at the onset of motion), then it must set the property 
     * in the navlib if the projection is orthographic or when it changes."),

        new("ViewRotatable"         , "bool"     , "bool", true  , false ,
@"     * Specifies whether the view can be rotated.
     *
     * This property is generally used to differentiate between orthographic 3D views and views that 
     * can only be panned and zoomed."),

        new("ViewTarget"            , "point"    , "FVector", true  , false ,
@"     * Specifies the target constraint of the view/camera.
     *
     * The camera target is the point in space the camera is constrained to look at by a ‘lookat’
     * controller attached to the camera. The side effects of the controller are that panning the 
     * constrained camera will also result in a camera rotation due to the camera being constrained to 
     * keep the target position in the center of the view. Similarly panning the target will result in the 
     * camera rotating."),

        new("ViewsFront"            , "matrix"   , "FMatrix", true  , false ,
@"     * Specifies the orientation of the view designated as the front view.
     *
     * The Navigation Library will only query the value of this property when the connection is
     * created. It is used to orientate the model to one of the 'Front', 'Back', 'Right', 'Left' etc.
     * views in response to the respective pre-defined view commands. If the orientation of the front
     * view is redefined after the connection is opened by the user, the client application is required
     * to update the property to the new value."),

        new("PivotPosition"         , "point"    , "FVector", true  , true  ,
@"     * The pivot_position_k property specifies the center of rotation of the model in world coordinates.
     *
     * This property is normally set by the navlib. The application can manually override the navlib 
     * calculated pivot and set a specific pivot position that the navlib will use until it is cleared again 
     * by the application."),

        new("PivotUser"             , "bool"     , "bool", true  , false ,
@"     * The pivot_user_k property specifies whether an application specified pivot is being used.
     *
     * To clear a pivot set by the application and to use the pivot algorithm in the navlib, the 
     * application sets this property to false. To override the navlib pivot algorithm the application can 
     * either set this property to true, which will cause the navlib to query the pivot position it should 
     * use, or the application can set the pivot position directly using the pivot_position_k property. The 
     * navlib’s pivot algorithm continues to be overridden until this property is set back to false."),

        new("PivotVisible"          , "bool"     , "bool", false , true  ,
@"     * The pivot_visible_k property specifies whether the pivot widget should be displayed.
     *
     * In the default configuration this property is set by the navlib to true when the user starts to move 
     * the model and to false when the user has finished moving the model."),

        new("HitLookfrom"           , "point"    , "FVector", false , true  ,
@"     * Defines the origin of the ray used for hit-testing in world coordinates.
     *
     * This property is set by the navlib"),

        new("HitDirection"          , "vector"   , "FVector", false , true  ,
@"     * Defines the direction of the ray used for hit-testing in world coordinates.
     *
     * This property is set by the navlib"),

        new("HitAperture"           , "float"    , "float", false , true  ,
@"     * Defines the diameter of the ray used for hit-testing.
     *
     * This property is set by the navlib"),

        new("HitLookat"             , "point"    , "FVector", true  , false ,
@"     * Specifies the point of the model that is hit by the ray originating from the lookfrom position.
     *
     * This property is queried by the navlib. The navlib will generally calculate if it is possible to hit a 
     * part of the model from the model_extents_k and selection_extents_k properties before setting up 
     * the hit-test properties and querying this property."),

        new("HitSelectionOnly"      , "bool"     , "bool", false , true  ,
@"     * Specifies whether the hit-testing is to be limited solely to the current selection set.
     *
     * This property is set by the navlib"),

        new("SelectionAffine"       , "matrix"   , "FMatrix", true  , true  ,
@"     * Specifies the matrix of the selection.
     *
     * This matrix specifies the object to world transformation of the selection. That is, multiplying this 
     * matrix on the right by the position (0, 0, 0) yields the position of the selection in world 
     * coordinates. The navlib will, generally, query this matrix at the beginning of a navigation action
     * that involves moving the selection and then set the property per frame. The frame rate that the 
     * navlib attempts to achieve is related to the 3D mouse event rate and is about 60Hz."),

        new("SelectionEmpty"        , "bool"     , "bool", true  , false , "     * When true, nothing is selected."),

        new("SelectionExtents"      , "box"      , "FBox", true  , false ,
@"     * Defines the bounding box of the selection in world coordinates
     *
     * This extents of the selection are returned as a bounding box in world coordinates. The navlib 
     * will only access this property if the selection_empty_k is false."),

        new("ModelExtents"          , "box"      , "FBox", true  , false , "     * Defines the bounding box of the model in world coordinates."),
        new("PointerPosition"       , "point"    , "FVector", true  , false ,
@"     * Defines the position of the mouse cursor on the projection plane in world coordinates.
     * The property is readonly.
     *
     * In OpenGL the position would typically be retrieved using gluUnProject with winZ set to 0.0."),
        // new("CommandsActiveSet"     , "string_t" , "FString"),
        // new("CommandsActiveCommand" , "string_t" , "FString"),
        // new("Settings"              , "string_t" , "FString"),
        // new("SettingsChanged"       , "long"     , "int"),
        // new("Images"                , "imagearray_t"      , "imagearray_t"),
        // new("CommandsTree"          , "SiActionNodeEx_t*" , "SiActionNodeEx_t*"),
    };

    public Target GenerateRuntimeNavlibInterface => _ => _
        .Unlisted()
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