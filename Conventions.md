# Conventions

## Basic guidelines to follow
1. Modular
    a) Systems not directly dependent on each other
        - ScriptableObject varaiables should be referenced to share data between systems
    b) Prefabs are self-contained
        - All logic should be within the prefab (as components)
        - GameEvents can be used to test external stuff
        - ScriptableObject references for external variables (even if something else populates the variables)
    c) Use Components that do only one thing
    d) Child gameobjects group by function/purpose?
2. Editable
    a) Change the game without code
        - ScriptableObject references can be changed in the editor
        - swap/modify component settings from editor to change behavior
3. Debuggable
    a) Test in isolation (modularity)
        - prefabs can be loaded into a scene with nothing else and work as expected
        - build test settings into prefab (if we need to load data to a ScriptableObject/init a GameEvent, etc.)

## Implementation
1. Base ScriptableObjects Types
    - [GameEvents & Listeners](Assets/GameEventSystem/ReadMe_GameEvents.md)
    -
2. SteamVR "Standalone Patch"
    - hacked by Dantman in 2019 to make SteamVR run as an overlay (the base of this whole project)
    - [Notes for Upgrading Steam VR](Assets/Scripts/SteamVR_Upgrade_Notes.md)



## Naming Conventions
### Inputs/Bindings/Controls/etc.
_Input Actions are designed 1to separate the logical meaning of an input from the physical means of input (that is, activity on an input device) that generate the input._
a) `Input Device`: physical peice of hardware
b) `Control`: separate, individual part of an input device. For example, an xbox controller's controls include, A/B/X/Y, two Joysticks, two triggers, etc.
c) `Interaction`: the ways to use a control. For example, You can click a joystick or move it in a direction. You can press, long press, double tap a button.
d) `Action`: A logical input such as "Select Target" or "Engage FSS"
e) `Binding`: A connection between an Action and one or more Controls represented by a control path. At run time, a Binding is resolved to yield zero or more Controls, which the Input System then connects to the Action.

7. SteamVR Controls vs vJoy Bound controls
    - only use SteamVR controls when a native binding isn't accessible. In other words: if you can map it by editing the Custom.X.0.binds file to a vJoy control, do it that way.
    Examples of good uses for SteamVR:
        - FSS Mode => Camera Activate
            - This control doesn't exist in the bindings file for Elite Dangerous
        - InteractUI => default SteamVR behavior and is used to do holographic button presses

****************************************

# T

<hr/>
# Structure (Maybe)
1. Monitors - check for information (bindings update, game state changes, etc.). Send Unity Events
    - File system watcher
    - Wrist turn watcher
    - Game status watcher
    -
2. Managers - non-unity states, assets, settings
    - InputBindingNameInfoManager
    - EDStateManager
    - EDControlBindings (EDControlManager)ED
    - vJoyInterface (vJoyManager)
        - SetButton
        - SetMapAxis
        - EnableMapAxis
        - IsDeviceValid()
        - Acquire Device
    - KeyboardInterface (KeyboardManager)
3. Controllers - interact with unity objects, control logic. Should be a monobehavior. Will often have an associated Manager
    - ActionsController?
    - vJoyInterface => move Monobehavior methods to a new class
    - KeyboardInterface => move Monobehavior methods to a new class
4. Interfaces - Helper functions for the primary type and map connections to other classes. Should **not** be a monobehavior class


5. GameObject Scripts - attached directly

**Maybe:**
1. Service - Loads/sends data from any backend services, Sends Events

