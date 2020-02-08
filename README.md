![Logo](Images/SensorKitLogo.gif)

[![openupm](https://img.shields.io/npm/v/ru.threedisevenzeror.sensorkit?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/ru.threedisevenzeror.sensorkit/)

Utility components for working with Raycasts using GameObjects

# What is this
This library wraps Physics.**X**Cast and Physics.Overlap**X** methods into MonoBehavior, and acts like a configurable sensor.
With that, you can write logic that is less focused on ray casting logic, and more on reaction when ray hit something.

# Features
* **Wraps all cast/overlap logic into separate class**
* **Multiple inheritance levels allows to write logic that is independent of sensor type (Cast or Overlap) and its shape (Ray, Sphere, Capsule, Box)**
* **Designed for embedding into existing logic**
* **Correctly applies all transforms, like collider with similar shape**
* **Highly detailed gizmo, so you can see how sensor acts**
* **Automatic method selection based on sensor settings**
* **Uses NonAlloc methods with reusable arrays for Multi-Object casts**
* **Since this is simple behavior, you can animate its values like any other object**

# Usage
- Add desired sensor to scene, and configure its detection params
   - **SphereCastSensor** | **BoxCastSensor** | **SphereOverlapSensor** | **BoxOverlapSensor**
- Create field in your behavior with same type, assign your sensor in inspector
- Call `UpdateSensor()` when you need to update your sensor
- Use obtained information any way you want

```CS
public class CharacterObject : MonoBehaviour
{
    public CastSensor groundSensor;

    public void Update()
    {
        groundSensor.UpdateSensor();
        
        if (groundSensor.HasHit)
        {
            var normal = groundSensor.RayHit.normal;
            // ground movement logic
        }
        else
        {
            // airborn movement logic
        }
    }
}
```

# Parameters
![Sensor parameters](Images/Params.png)

Nearly all of sensor parameters are actually Cast/Overlap method parameters, so i think if you need documentation for those, you can easily read detailed info in Unity Documentation. However, there is unique parameters:
- **Max Results** - Since sensor can detect multiple objects at once, it needs to preallocate array for specific object count, to avoid garbage generation for each cast.
- **Lazy Allocation** - If there is chance that your sensor will be unused, you may tick this checkbox, to delay array allocation untill first use. This option may save you few nanoseconds, if you are desperate for performance.
- **Ray Direction Override** - For Cast* sensors, ability to separate ray cast direction from sensor rotation.


# Gizmo
Each sensor has detailed gizmo, to aid sensor configuration and allow quick visualization of current sensor behavior

_Gizmo is visible only on selected game objects for peformance reasons_
![Gizmo](Images/Gizmo.png)

Colors:
- **Red gizmo**: Nothing Detected
- **Green line**: Distance that cast has moved untill detection
- **Green dot**: Point of object stop distance
- **Yellow line**: Leftover distance, that cast could reach if there were no collision
- **Blue arrow**: Hit normal
- **Purple point**: Actual hit position
- **White triangle**: Highlight of the triangle that was hit (If it is mesh collider)

# Architecture
This library is designed to allow faster Cast/Overlap iteration cycles, and give ability to easily swap between sensor types without code modification.

![Class Diagram](Images/ClassDiagram.png)
###### Ray/Capsule casts are absent because they are SphereCast's special cases. If Radius = 0, Ray cast is used for detection, if Width > 0, Capsule cast is used. Similar logic applies to overlap sensors.

Every inheritance level has logic that allows to work with results independently of sensor type. So, if you just need to check for object prescence, you can use base class - PhysicsSensor just to get basic information about hit (Is detected something, Collider of detected object). With that, you can assign any sensor to that property, and if you need to change from BoxOverlap to SphereCast, you dont even need to modify your sources, you can just swap your sensor.

Sensor **is not updated automatically**, you need to call `UpdateSensor()` for that. Here is reasons:
- Every game object uses casts in his own lifecycle, which can be in update/fixed update/coroutine/whatever
- There is no guarantee that sensor will be used every frame
- Often you want to keep last cast information untill next update

# Requirements
At least Unity **2018.1** or higher. **2019.1** is required for overriding PhysicsScene

# Installation

* **OpenUPM**: 
    The package is available on the [openupm registry](https://openupm.com). It's recommended to install it via [openupm-cli](https://github.com/openupm/openupm-cli).
    ```
    openupm add com.littlebigfun.addressable-importer
    ```
* **Git (2018.3+)**: 
    If git is installed, you can add following entry in **Packages\manifest.json**
    ```
    "ru.threedisevenzeror.sensorkit": "https://github.com/3DI70R/SensorKit.git"
    ```
    And all these scripts will be included in your project as dependency.
* **Local Package (2018.3+):**
    Just download this repository as ZIP, extract, and then add it via "Add package from disk..." from Package Manager

Otherwise, you can just copy files from `Assets/Scripts/Runtime` to your project.

# Changelog
* 1.0.0
    - Initial release
* 1.1.0
    - Feature: Ability to override PhysicsScene used by sensors (only for Unity 2019.1+)
    - Feature: Ability to override ray cast direction to allow non ray aligned shapes
    - Bugfix: RayCast/SphereCast/CapsuleCast didn't used provided precalculated Ray instance and calculated its own version (which is identical, so it is just small performance fix)
* 1.1.1
    - Bugfix: null pointer exception while gizmos are enabled in game view
* 1.1.2
    - Bugfix: CastSensor stored rayHits in **static** array. This is a big glaring bug that i didn't noticed. Sorry. (Basically, you lose cast information for previous sensors if you didn't used it right away)
    - Bugfix: Text was slightly clipped in 2019.3 due to UI update
* 1.1.3
    - Improvement: Documentation formatting fixes, Tooltips for editor
    - Improvement: Hit information window style update, also now it doesn't clip out of view
    - Improvement: Vertex caching for hit information overlay
    - Improvement: "Lazy Allocation" now readonly, non keyable and can be set only from inspector, because it is used only during component initialization
